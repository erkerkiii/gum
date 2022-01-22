using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gum.WebRequest
{
    public sealed class GumWebRequest
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        private readonly HttpRequestMessage _httpRequestMessage;

        public bool IsDone { get; private set; }
        
        public Response Result { get; private set; }
        
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

        public async Task<Response> Send()
        {
            Task<HttpResponseMessage> response = HttpClient.SendAsync(_httpRequestMessage);
            await response;
            HttpResponseMessage responseMessage = response.Result;
            
            string text = await responseMessage.Content.ReadAsStringAsync();

            IsDone = true;

            Result = new Response((int)responseMessage.StatusCode, text);
            return Result;
        }

        public readonly struct Response
        {
            public readonly int StatusCode;
            
            public readonly string Text;
            
            internal Response(int statusCode, string text)
            {
                StatusCode = statusCode;
                Text = text;
            }
        }
    }
}