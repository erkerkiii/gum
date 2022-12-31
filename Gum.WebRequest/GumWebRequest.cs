﻿using System;
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
        
        private CancellationTokenSource _cancellationTokenSource;
        
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

        public async Task<Response> SendAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            HttpResponseMessage responseMessage =
                await HttpClient.SendAsync(_httpRequestMessage, _cancellationTokenSource.Token);
            
            downloadHandle.Status = Status.OnGoing;

            if (!responseMessage.IsSuccessStatusCode)
            {
                downloadHandle.Error = $"Failed with code: {responseMessage.StatusCode}";
                downloadHandle.Status = Status.Fail;
                downloadHandle.IsDone = true;
                return null;
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
                           .ReadAsync(buffer, 0, BUFFER_SIZE, _cancellationTokenSource.Token)
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

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void Dispose()
        {
            _httpRequestMessage?.Dispose();
            _cancellationTokenSource?.Dispose();
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