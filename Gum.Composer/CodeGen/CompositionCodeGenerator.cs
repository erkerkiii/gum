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
		private const string ASPECT_CATALOG_FILE = "AspectTypeCatalog.cs";
		private const string TAB = "\t";
		private const string LINE = "\n";
		private const string ASPECT_TYPE = "AspectType";
		private const string ASPECT_CATALOG_CLASS = "AspectTypeCatalog";
		private const string TYPE = "$type";
		private const string FIELD_NAME = "$fieldName";
		private const string NAMESPACE = "$namespace";
		private const string OBJECT_NAME = "$objName";
		private const string CONTENT = "$content";
		private const string ARGS = "$args";
		private const string CTOR_CONTENT = "$ctorContent";
		private const string ASPECT_ORDER = "$aspectOrder";
		private const string NAMESPACE_TEMPLATE = "namespace " + NAMESPACE + ".Generated";
		private const string ASPECT_CATALOG_TEMPLATE = NAMESPACE_TEMPLATE +
		                                     LINE + "{" +
		                                     LINE + TAB + "public static class " + ASPECT_CATALOG_CLASS +
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
			TAB + TAB + "public static readonly " + ASPECT_TYPE + " ASPECT_TYPE = " + ASPECT_CATALOG_CLASS + "." + OBJECT_NAME + ";" +
			LINE + LINE +
			TAB + TAB + "public " + ASPECT_TYPE + " Type => ASPECT_TYPE;" + LINE + LINE;
		private const string FIELD_TEMPLATE =
			TAB + TAB + "public readonly " + TYPE + " " + FIELD_NAME + ";" + LINE + LINE;
		private const string ASPECT_CATALOG_FIELD_TEMPLATE =
			TAB + TAB + "public const System.Int32 " + TYPE + " = " + ASPECT_ORDER + ";" + LINE;
		
		public static void Run()
		{
			int createdAspectIndex = 0;

			StringBuilder aspectFileStringBuilder = new StringBuilder();
			StringBuilder bodyStringBuilder = new StringBuilder();
			StringBuilder argsStringBuilder = new StringBuilder();
			StringBuilder aspectTypeCatalogStringBuilder = new StringBuilder();
			StringBuilder ctorStringBuilder = new StringBuilder();
			aspectFileStringBuilder.Append(NAMESPACE_TEMPLATE.Replace(NAMESPACE, PathConfig.NAMESPACE));
			aspectFileStringBuilder.Append(LINE + "{");
			foreach (AspectPrototype aspectPrototype in AspectFileReader.ReadAspects())
			{
				string aspectName = aspectPrototype.Name;
				bodyStringBuilder.Append(ASPECT_TYPE_TEMPLATE
					.Replace(OBJECT_NAME, aspectName));
				
				int argCounter = 0;
				foreach (KeyValuePair<string, string> kvp in aspectPrototype.Fields)
				{
					string fieldName = kvp.Key;
					string type = kvp.Value;
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

				aspectTypeCatalogStringBuilder.Append(ASPECT_CATALOG_FIELD_TEMPLATE
					.Replace(TYPE, aspectName)
					.Replace(ASPECT_ORDER, createdAspectIndex.ToString()));
				createdAspectIndex++;
				
				bodyStringBuilder.Clear();
				argsStringBuilder.Clear();
				ctorStringBuilder.Clear();
			}

			aspectFileStringBuilder.Append(LINE + "}");

			string aspectCatalogClassFile = Path.Combine(PathConfig.GetOutputDirectoryPath(), ASPECT_CATALOG_FILE);
			string aspectsFile = Path.Combine(PathConfig.GetOutputDirectoryPath(), ASPECTS_FILE);

			FilePathHelper.EnsureFilePath(aspectCatalogClassFile);
			FilePathHelper.EnsureFilePath(aspectsFile);

			File.WriteAllText(aspectCatalogClassFile, ASPECT_CATALOG_TEMPLATE
				.Replace(NAMESPACE, PathConfig.NAMESPACE)
				.Replace(CONTENT, aspectTypeCatalogStringBuilder.ToString()));
			File.WriteAllText(aspectsFile, aspectFileStringBuilder.ToString());
		}
	}
}