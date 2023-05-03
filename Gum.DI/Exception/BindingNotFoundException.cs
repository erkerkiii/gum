using Gum.Core.Assert;

namespace Gum.DI.Exception
{
	public class BindingNotFoundException : GumException
	{
		public BindingNotFoundException(string message) : base(message)
		{
		}
	}
}