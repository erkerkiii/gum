using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Gum.Composition.CodeGen.Internal;

namespace Gum.Composition.CodeGen
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
			TAB + TAB + "public const " + ASPECT_TYPE + " ASPECT_TYPE = " + ASPECT_TYPE + "." + OBJECT_NAME + ";" +
			LINE + LINE +
			TAB + TAB + "public " + ASPECT_TYPE + " Type => ASPECT_TYPE;" + LINE + LINE;
		private const string FIELD_TEMPLATE =
			TAB + TAB + "public readonly " + TYPE + " " + FIELD_NAME + ";" + LINE + LINE;

		public static async Task RunAsync()
		{
			StringBuilder aspectFileStringBuilder = new StringBuilder();
			StringBuilder bodyStringBuilder = new StringBuilder();
			StringBuilder argsStringBuilder = new StringBuilder();
			StringBuilder aspectTypeEnumStringBuilder = new StringBuilder();
			StringBuilder ctorStringBuilder = new StringBuilder();

			aspectFileStringBuilder.Append(NAMESPACE_TEMPLATE.Replace(NAMESPACE, UserConfig.NAMESPACE));
			aspectFileStringBuilder.Append(LINE + "{");
			foreach (AspectPrototype aspectPrototype in await AspectFileReader.ReadAspectsAsync())
			{
				string aspectName = aspectPrototype.Name;
				bodyStringBuilder.Append(ASPECT_TYPE_TEMPLATE
					.Replace(OBJECT_NAME, aspectName));
				int argCounter = 0;
				foreach ((string type, string fieldName) in aspectPrototype.Fields)
				{
					bodyStringBuilder.Append(FIELD_TEMPLATE
						.Replace(TYPE, type)
						.Replace(FIELD_NAME, fieldName));

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

			const string aspectTypeEnumFile = UserConfig.OUTPUT_DIRECTORY_PATH + ASPECT_TYPE_FILE;
			const string aspectsFile = UserConfig.OUTPUT_DIRECTORY_PATH + ASPECTS_FILE;

			EnsureFilePath(aspectTypeEnumFile);
			EnsureFilePath(aspectsFile);

			await Task.WhenAll(
				File.WriteAllTextAsync(aspectTypeEnumFile, ENUM_TEMPLATE
					.Replace(NAMESPACE, UserConfig.NAMESPACE)
					.Replace(CONTENT, aspectTypeEnumStringBuilder.ToString())),
				File.WriteAllTextAsync(aspectsFile, aspectFileStringBuilder.ToString()));
		}

		private static void EnsureFilePath(string filePath)
		{
			if (!File.Exists(filePath))
			{
				File.Create(filePath).Close();
			}
		}
	}
}