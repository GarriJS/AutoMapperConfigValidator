using AutoMapperConfigValidator.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoMapperConfigValidator
{
	public static class AutoMapperMapExtractorUtil
	{
		public static IEnumerable<AutoMapperMap> ExtractMapInfosFromFile(string[] filePaths)
		{
			if (filePaths == null)
			{
				yield break;
			}

			foreach (var filePath in filePaths)
			{
				foreach (var type in ExtractMapInfosFromFile(filePath))
				{
					yield return type;
				}
			}
		}

		public static IEnumerable<AutoMapperMap> ExtractMapInfosFromFile(string filePath)
		{
			var text = File.ReadAllText(filePath);
			var tree = CSharpSyntaxTree.ParseText(text);
			var root = tree.GetRoot();
			var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();

			foreach (var invocation in invocations)
			{
				if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
					memberAccess.Name is GenericNameSyntax genericName &&
					Configuration.MapFunctionNames.Contains(genericName.Identifier.Text, StringComparer.OrdinalIgnoreCase))
				{
					var typeOut = genericName.TypeArgumentList.Arguments[0].ToString();
					var sourceVariableName = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression.ToString() ?? "unknown";
					var lineNumber = invocation.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
					var normalizedTypeOut = new string([.. typeOut.Where(c => !char.IsWhiteSpace(c))]);

					if (Configuration.TypeOutContainsMuteList.Any(mute => normalizedTypeOut.Contains(mute.Replace(" ", ""))))
					{
						continue;
					}

					var autoMapperMapInfo = new AutoMapperMap
					{
						TypeOut = typeOut,
						SourceVariableName = sourceVariableName,
						FilePath = filePath,
						LineNumber = lineNumber
					};

					yield return autoMapperMapInfo;
				}
			}
		}
	}
}
