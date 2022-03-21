using System;
using Gum.WebRequest;

namespace Gum.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            using (GumWebRequest gumWebRequest = GumWebRequest.Get("https://www.google.com"))
            {
                gumWebRequest.SendAsync();
                while (!gumWebRequest.IsDone)
                {
                    Console.WriteLine($"{gumWebRequest.Progress}");
                }
            }
        }
    }
}