using Gum.Core.Assert;

namespace Gum.DI.Exception
{
	public class BindingAlreadyExistsException : GumException
	{
		public BindingAlreadyExistsException(string message) : base(message)
		{
		}
	}
}