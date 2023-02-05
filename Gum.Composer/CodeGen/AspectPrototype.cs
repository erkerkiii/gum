using System.Collections.Generic;

namespace Gum.Composer.CodeGen
{
	public readonly struct AspectPrototype
	{
		public readonly string Name;
			
		public readonly Dictionary<string, string> Fields;

		public AspectPrototype(string name, Dictionary<string, string> fields)
		{
			Name = name;
			Fields = fields;
		}
	}
}