using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Gum.Drafter
{
    public sealed class MethodToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter is not string methodName)
            {
                return null;
            }
            
            MethodInfo methodInfo = value.GetType().GetMethod(methodName, Type.EmptyTypes);
            return methodInfo == null
                ? null
                : methodInfo.Invoke(value, Array.Empty<object>());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{GetType().Name} can only be used for one way conversion.");
        }
    }
}