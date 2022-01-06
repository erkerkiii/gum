namespace Gum.Core.Assert
{
    public static class GumAssert
    {
        public static void NotNull(object obj)
        {
            if (obj == null)
            {
                CreateException($"Assert hit! {nameof(obj)} is null!");
            }
        }

        public static void AreEqual(object first, object second)
        {
            if (!Equals(first, second))
            {
                CreateException();
            }
        }
        
        public static void IsTrue(bool value)
        {
            if (!value)
            {
                CreateException();
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
            throw new GumException(message);
        }

        private static void CreateException()
        {
            throw new GumException("Assert hit!");
        }
    }
}