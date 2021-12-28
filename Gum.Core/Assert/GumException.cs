using System;

namespace Gum.Core.Assert
{
    public class GumException : Exception
    {
        public GumException()
        {
        }

        public GumException(string message) : base(message)
        {
        }
    }
}