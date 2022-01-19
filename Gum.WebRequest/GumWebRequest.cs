using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gum.WebRequest
{
    public class GumWebRequest : IDisposable
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public readonly Dictionary<string, string> headers = new Dictionary<string, string>();

        public async Task<Response> Get(string url)
        {
            using (HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url))
            {
                int headerCount = headers.Count;
                for (int index = 0; index < headerCount; index++)
                {
                    KeyValuePair<string, string> keyValuePair = headers.ElementAt(index);
                    httpRequestMessage.Headers.Add(keyValuePair.Key, keyValuePair.Value);
                }

                Task<HttpResponseMessage> response = HttpClient.SendAsync(httpRequestMessage);
                await response;
                string text = await response.Result.Content.ReadAsStringAsync();
                return new Response((int)response.Result.StatusCode, text);
            }
        }
        
        public async Task<Response> Post(string url, string content = null)
        {
            using (HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url))
            {
                if (!string.IsNullOrEmpty(content))
                {
                    httpRequestMessage.Content = new StringContent(content);
                }
                
                int headerCount = headers.Count;
                for (int index = 0; index < headerCount; index++)
                {
                    KeyValuePair<string, string> keyValuePair = headers.ElementAt(index);
                    httpRequestMessage.Headers.Add(keyValuePair.Key, keyValuePair.Value);
                }

                Task<HttpResponseMessage> response = HttpClient.SendAsync(httpRequestMessage);
                await response;
                string text = await response.Result.Content.ReadAsStringAsync();
                return new Response((int)response.Result.StatusCode, text);
            }
        }

        public void Dispose()
        {
            headers.Clear();
            GC.SuppressFinalize(this);
        }
        
        public readonly struct Response
        {
            public readonly int StatusCode;
            
            public readonly string Text;

            public Response(int statusCode, string text)
            {
                StatusCode = statusCode;
                Text = text;
            }
        }
    }
}