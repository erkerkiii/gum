namespace Gum.Core.Assert
{
    public static class GumAssert
    {
        public static void NotNull(object obj, string message = null)
        {
            if (obj == null)
            {
                CreateException(string.IsNullOrEmpty(message)
                    ? $"{nameof(obj)} is null!"
                    : message);
            }
        }

        public static void AreEqual(object first, object second, string message = null)
        {
            if (!Equals(first, second))
            {
                CreateException(string.IsNullOrEmpty(message)
                    ? $"{first} and {second} are not equal!"
                    : message);
            }
        }
        
        public static void IsTrue(bool value, string message = null)
        {
            if (!value)
            {
                CreateException(string.IsNullOrEmpty(message)
                    ? "The given value is not true!"
                    : message);
            }
        }

        public static void IsFalse(bool value, string message = null)
        {
            if (value)
            {
                CreateException(string.IsNullOrEmpty(message)
                    ? "The given value is not true!"
                    : message);
            }
        }
        
        public static void GreaterThanZero(int value)
        {
            if (value < 1)
            {
                CreateException();
            }
        }
        
        private static void CreateException(string message)
        {
            throw new GumException($"Assert hit! {message}");
        }

        private static void CreateException()
        {
            throw new GumException("Assert hit!");
        }
    }
}