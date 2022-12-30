using System.IO;
using System.Text;

namespace Gum.Sandbox
{
	class Program
	{
		private const string TYPE = "TYPE";
		private const string ARG = "ARGS";
		private const string ARGS_IMPL = "ARGS_IMPL";

		static void Main(string[] args)
		{
			const string path = @"C:\Users\Nihan\RiderProjects\gum\Gum.Pooling\StackPool.cs";
			string pool = File.ReadAllText(@"C:\Users\Nihan\RiderProjects\gum\Gum.Pooling\tmplt.txt");
			StringBuilder pathString = new StringBuilder(File.ReadAllText(path));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 16; i++)
			{
				if (i == 0)
				{
					stringBuilder.Append(pool.Replace(TYPE, "").Replace(ARGS_IMPL, "").Replace(ARG, ""));
					continue;
				}

				string arg = string.Empty;
				string argImpl = string.Empty;
				string type = string.Empty;
				for (int j = 0; j < i; j++)
				{
					var t = $"T{j + 1}";
					if (j > 0)
					{
						arg += ", ";
						type += ", ";
						argImpl += ", ";
					}

					type += $"{t}";
					arg += $"{t} arg{j}";
					argImpl += $"arg{j}";
				}

				stringBuilder.Append(pool.Replace(TYPE, type).Replace(ARGS_IMPL, argImpl).Replace(ARG, arg));
			}

			File.WriteAllText(path, stringBuilder.ToString());
		}
	}
}