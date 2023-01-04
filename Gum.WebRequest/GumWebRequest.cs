using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gum.WebRequest
{
    public sealed class GumWebRequest : IDisposable
    {
        private const int BUFFER_SIZE = 8192;
        
        private static readonly HttpClient HttpClient = new HttpClient();

        private readonly HttpRequestMessage _httpRequestMessage;
        
        private CancellationTokenSource _timeoutTokenSource;
        
        public DownloadHandle downloadHandle { get; }
        
        private GumWebRequest(HttpRequestMessage httpRequestMessage)
        {
            _httpRequestMessage = httpRequestMessage;
            downloadHandle = new DownloadHandle();
        }

        public void AddHeader(string key, string value)
        {
            _httpRequestMessage.Headers.Add(key, value);
        }
        
        public void AddHeader(string key, IEnumerable<string> value)
        {
            _httpRequestMessage.Headers.Add(key, value);
        }

        public static GumWebRequest Get(string url)
        {
            return new GumWebRequest(new HttpRequestMessage(HttpMethod.Get, url));
        }
        
        public static GumWebRequest Post(string url, HttpContent httpContent = null)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequestMessage.Content = httpContent;
            
            return new GumWebRequest(httpRequestMessage);
        }
        
        public static GumWebRequest Put(string url, HttpContent httpContent = null)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, url);
            httpRequestMessage.Content = httpContent;
            
            return new GumWebRequest(httpRequestMessage);
        }

        public async Task<Response> SendAsync(CancellationToken rCancellationToken = default, TimeSpan timeoutSpan = default)
        {
            if (timeoutSpan == default)
            {
                timeoutSpan = HttpClient.Timeout;
            }

            _timeoutTokenSource = new CancellationTokenSource(timeoutSpan);
            
            CancellationToken timeOutCancellationToken = _timeoutTokenSource.Token;
            CancellationToken cancellationToken = timeOutCancellationToken;
            if (rCancellationToken != default)
            {
                cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(timeOutCancellationToken, rCancellationToken).Token;
            }

            try
            {
                HttpResponseMessage responseMessage = await HttpClient.SendAsync(_httpRequestMessage, cancellationToken);
                            
                downloadHandle.Status = Status.OnGoing;
            
                if (!responseMessage.IsSuccessStatusCode)
                {
                    downloadHandle.Error = $"Failed with code: {responseMessage.StatusCode}";
                    downloadHandle.Status = Status.Fail;
                    downloadHandle.IsDone = true;
                
                    return new Response((int)responseMessage.StatusCode, Array.Empty<byte>());
                }

                long? contentLength = responseMessage.Content.Headers.ContentLength;
                if (contentLength.HasValue)
                {
                    downloadHandle.TotalBytesToReceive = contentLength.Value;
                }
            
                byte[] buffer = new byte[BUFFER_SIZE];
                List<byte> bytes = new List<byte>();
                using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync())
                {
                    int bytesRead;
                    while ((bytesRead = await responseStream
                               .ReadAsync(buffer, 0, BUFFER_SIZE, cancellationToken)
                               .ConfigureAwait(false)) > 0)
                    {
                        bytes.AddRange(buffer.Take(bytesRead));
                        downloadHandle.BytesReceived += bytesRead;
                    
                        if (downloadHandle.TotalBytesToReceive > 0)
                        {
                            downloadHandle.Progress = (float)downloadHandle.BytesReceived / (float)downloadHandle.TotalBytesToReceive;
                        }
                    }
                }
            
                downloadHandle.Status = Status.Success;
                downloadHandle.Result = new Response((int)responseMessage.StatusCode, bytes.ToArray());
                downloadHandle.IsDone = true;
                return downloadHandle.Result;
            }
            catch (Exception exception)
            {
                const int notImplementedStatusCode = 501;
                const int timeOutResponseCode = 408;

                if (timeOutCancellationToken.IsCancellationRequested)
                {
                    return new Response(timeOutResponseCode, Array.Empty<byte>());
                }

                if (!(exception is WebException webException) || !(webException.Response is HttpWebResponse httpWebResponse))
                {
                    return new Response(notImplementedStatusCode, Array.Empty<byte>());
                }

                return new Response((int)httpWebResponse.StatusCode, Array.Empty<byte>());
            }
        }

        public void Cancel()
        {
            _timeoutTokenSource?.Cancel();
        }

        public void Dispose()
        {
            _httpRequestMessage?.Dispose();
            _timeoutTokenSource?.Dispose();
        }
        
        public sealed class Response : IDisposable
        {
            public readonly int StatusCode;
            
            public byte[] Data { get; private set; }

            private string _cachedText;
            
            public string Text
            {
                get
                {
                    if (string.IsNullOrEmpty(_cachedText))
                    {
                        _cachedText = Encoding.UTF8.GetString(Data);
                    }

                    return _cachedText;
                }
            }

            internal Response(int statusCode, byte[] data)
            {
                StatusCode = statusCode;
                Data = data;
            }

            public void Dispose()
            {
                Data = null;
            }
        }
        
        public sealed class DownloadHandle
        {
            public float Progress { get; internal set; }
        
            public long BytesReceived { get; internal set; }
            public long TotalBytesToReceive { get; internal set; }
            
            public Response Result { get; internal set; }

            public bool IsDone { get; internal set; }
            
            public string Error { get; internal set; }

            public Status Status { get; internal set; } = Status.None;
        }

        public enum Status : byte
        {
            None,
            OnGoing,
            Success,
            Fail
        }
    }
}