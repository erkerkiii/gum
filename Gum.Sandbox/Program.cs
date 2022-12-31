using System.Threading;
using System.Threading.Tasks;
using Gum.Composition.CodeGen;

namespace Gum.Sandbox
{
	class Program
	{
		private  static bool isCompleted;
		
		static void Main(string[] args)
		{
			StartAsync();
			while (!isCompleted)
			{
				Thread.Sleep(10);
			}
		}

		private static async Task StartAsync()
		{
			await CompositionCodeGenerator.RunAsync();
			isCompleted = true;
		}
	}
}