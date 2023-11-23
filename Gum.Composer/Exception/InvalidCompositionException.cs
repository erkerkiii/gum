using Gum.Core.Assert;

namespace Gum.Composer.Exception
{
	public class InvalidCompositionException : GumException
	{
		public InvalidCompositionException(string message) : base(message)
		{
		}
	}
}