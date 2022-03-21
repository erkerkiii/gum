using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public Response Result { get; private set; }

        public bool IsDone { get; private set; }
        
        public float Progress { get; private set; }
        
        public long BytesReceived { get; private set; }
        public long TotalBytesToReceive { get; private set; }
        

        private CancellationTokenSource _cancellationTokenSource;
        
        private GumWebRequest(HttpRequestMessage httpRequestMessage)
        {
            _httpRequestMessage = httpRequestMessage;
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

        public async Task<Response> SendAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            HttpResponseMessage responseMessage =
                await HttpClient.SendAsync(_httpRequestMessage, _cancellationTokenSource.Token);

            long? contentLength = responseMessage.Content.Headers.ContentLength;
            if (contentLength.HasValue)
            {
                TotalBytesToReceive = contentLength.Value;
            }
            
            byte[] buffer = new byte[BUFFER_SIZE];
            List<byte> bytes = new List<byte>();
            using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync())
            {
                int bytesRead;
                while ((bytesRead = await responseStream
                           .ReadAsync(buffer, 0, BUFFER_SIZE, _cancellationTokenSource.Token).ConfigureAwait(false)) > 0)
                {
                    bytes.AddRange(buffer.Take(bytesRead));
                    BytesReceived += bytesRead;
                    Progress = (float)BytesReceived / (float)TotalBytesToReceive;
                }
            }

            IsDone = true;
            Result = new Response((int)responseMessage.StatusCode, bytes.ToArray());
            return Result;
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void Dispose()
        {
            _httpRequestMessage?.Dispose();
            Result?.Dispose();
            _cancellationTokenSource?.Dispose();

            BytesReceived = 0;
            TotalBytesToReceive = 0;
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
                        _cachedText = Encoding.Default.GetString(Data);
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
    }
}