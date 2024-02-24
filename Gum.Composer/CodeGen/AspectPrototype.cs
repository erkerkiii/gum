using System.Collections.Generic;

namespace Gum.Composer.CodeGen
{
	public readonly struct AspectPrototype
	{
		public readonly string Name;
			
		public readonly Dictionary<string, string> Fields;

		public bool IsTagAspect => Fields.Count < 1;

		public AspectPrototype(string name, Dictionary<string, string> fields)
		{
			Name = name;
			Fields = fields;
		}
	}
}