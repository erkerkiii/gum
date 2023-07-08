using System.Collections.Generic;
using System.IO;
using System.Text;
using Gum.Composer.CodeGen.Internal;
using Gum.Composer.Utility;

namespace Gum.Composer.CodeGen
{
	public static class CompositionCodeGenerator
	{
		private const string ASPECTS_FILE = "Aspects.cs";
		private const string ASPECT_TYPE_FILE = "AspectType.cs";
		private const string TAB = "\t";
		private const string LINE = "\n";
		private const string ASPECT_TYPE = "AspectType";
		private const string TYPE = "$type";
		private const string FIELD_NAME = "$fieldName";
		private const string NAMESPACE = "$namespace";
		private const string OBJECT_NAME = "$objName";
		private const string CONTENT = "$content";
		private const string ARGS = "$args";
		private const string CTOR_CONTENT = "$ctorContent";
		private const string NAMESPACE_TEMPLATE = "namespace " + NAMESPACE + ".Generated";
		private const string ENUM_TEMPLATE = NAMESPACE_TEMPLATE +
		                                     LINE + "{" +
		                                     LINE + TAB + "public enum " + ASPECT_TYPE +
		                                     LINE + TAB + "{" +
		                                     LINE + CONTENT +
		                                     TAB + "}"+
		                                     LINE + "}";
		private const string STRUCT_TEMPLATE = LINE + TAB + "public readonly struct " + OBJECT_NAME + " : IAspect" +
		                                       LINE + TAB + "{" +
		                                       LINE + CONTENT +
		                                       LINE + TAB + "}";
		private const string CTOR_TEMPLATE = TAB + TAB + "public " + OBJECT_NAME + "Aspect(" + ARGS + ")" +
		                                     LINE + TAB + TAB + "{" +
		                                     TAB + TAB + LINE + CTOR_CONTENT +
		                                     LINE + TAB + TAB + "}";
		private const string ASPECT_TYPE_TEMPLATE =
			TAB + TAB + "public static readonly " + NAMESPACE + "." + ASPECT_TYPE + " ASPECT_TYPE = " + "(int)" + ASPECT_TYPE + "." + OBJECT_NAME + ";" +
			LINE + LINE +
			TAB + TAB + "public " + NAMESPACE + "." + ASPECT_TYPE + " Type => ASPECT_TYPE;" + LINE + LINE;
		private const string FIELD_TEMPLATE =
			TAB + TAB + "public readonly " + TYPE + " " + FIELD_NAME + ";" + LINE + LINE;

		public static void Run()
		{
			StringBuilder aspectFileStringBuilder = new StringBuilder();
			StringBuilder bodyStringBuilder = new StringBuilder();
			StringBuilder argsStringBuilder = new StringBuilder();
			StringBuilder aspectTypeEnumStringBuilder = new StringBuilder();
			StringBuilder ctorStringBuilder = new StringBuilder();

			aspectFileStringBuilder.Append(NAMESPACE_TEMPLATE.Replace(NAMESPACE, UserConfig.NAMESPACE));
			aspectFileStringBuilder.Append(LINE + "{");
			foreach (AspectPrototype aspectPrototype in AspectFileReader.ReadAspects())
			{
				string aspectName = aspectPrototype.Name;
				bodyStringBuilder.Append(ASPECT_TYPE_TEMPLATE
					.Replace(OBJECT_NAME, aspectName)
					.Replace(NAMESPACE, UserConfig.NAMESPACE));
				
				int argCounter = 0;
				foreach (KeyValuePair<string, string> kvp in aspectPrototype.Fields)
				{
					string fieldName = kvp.Key;
					string type = kvp.Value;
					bodyStringBuilder.Append(FIELD_TEMPLATE
						.Replace(TYPE, type)
						.Replace(FIELD_NAME, fieldName)
						.Replace(NAMESPACE, UserConfig.NAMESPACE));

					string argName = $"arg{argCounter}";
					argsStringBuilder.Append($"{type} {argName}, ");
					ctorStringBuilder.Append($"{LINE}{TAB}{TAB}{TAB} {fieldName} = {argName};");
					argCounter++;
				}

				argsStringBuilder.Remove(argsStringBuilder.Length - 2, 2);

				bodyStringBuilder.Append(CTOR_TEMPLATE.Replace(OBJECT_NAME, aspectName)
					.Replace(ARGS, argsStringBuilder.ToString())
					.Replace(CTOR_CONTENT, ctorStringBuilder.ToString()));
				aspectFileStringBuilder.Append(STRUCT_TEMPLATE
					.Replace(OBJECT_NAME, $"{aspectName}Aspect")
					.Replace(CONTENT, bodyStringBuilder.ToString()));

				aspectTypeEnumStringBuilder.Append($"{TAB + TAB}{aspectName},{LINE}");

				bodyStringBuilder.Clear();
				argsStringBuilder.Clear();
				ctorStringBuilder.Clear();
			}

			aspectFileStringBuilder.Append(LINE + "}");

			string aspectTypeEnumFile = $@"{UserConfig.OutputDirectoryPath}\{ASPECT_TYPE_FILE}";
			string aspectsFile = $@"{UserConfig.OutputDirectoryPath}\{ASPECTS_FILE}";

			FilePathHelper.EnsureFilePath(aspectTypeEnumFile);
			FilePathHelper.EnsureFilePath(aspectsFile);

			File.WriteAllText(aspectTypeEnumFile, ENUM_TEMPLATE
				.Replace(NAMESPACE, UserConfig.NAMESPACE)
				.Replace(CONTENT, aspectTypeEnumStringBuilder.ToString()));
			File.WriteAllText(aspectsFile, aspectFileStringBuilder.ToString());
		}
	}
}