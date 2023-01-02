using System.Threading;
using System.Threading.Tasks;
using Gum.Composer.CodeGen;

namespace Gum.Sandbox
{
	class Program
	{
		private static bool _isCompleted;

		static void Main(string[] args)
		{
			RunAsync();
			while (!_isCompleted)
			{
				Thread.Sleep(10);
			}
		}

		private static async Task RunAsync()
		{
			await CompositionCodeGenerator.RunAsync();
			_isCompleted = true;
		}
	}
}