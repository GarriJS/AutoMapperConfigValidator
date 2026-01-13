using AutoMapperConfigValidator.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoMapperConfigValidator
{
	public static class CompileMappingConfigurationsUtil
	{
		public static IEnumerable<AutoMapperConfig> ExtractCreateMapTypes(string[] filePaths)
		{
			foreach (var filePath in filePaths)
			{
				var text = File.ReadAllText(filePath);
				var tree = CSharpSyntaxTree.ParseText(text);
				var root = tree.GetRoot();

				foreach (var invocation in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
				{
					if (!TryGetCreateMap(invocation, out var source, out var destination))
					{
						continue;
					}

                    var typeIn = source.Split('.').LastOrDefault()
                                       ?.Replace(" ", string.Empty);

                    var typeOut = destination.Split('.').LastOrDefault()
                                             ?.Replace(" ", string.Empty);

                    if (typeIn == null ||
						typeOut == null)
					{
						continue;
					}

                    var autoMapperConfig = new AutoMapperConfig
					{
						TypeIn = typeIn!,
						TypeOut = typeOut!,
						IsReverseMap = false,
					};

					yield return autoMapperConfig;

					if (HasReverseMap(invocation))
					{
						var reverseAutoMapperConfig = new AutoMapperConfig
						{
							TypeIn = typeOut!,
							TypeOut = typeIn!,
							IsReverseMap = true,
						};

						yield return reverseAutoMapperConfig;
					}
				}
			}
		}

        private static bool TryGetCreateMap(
            InvocationExpressionSyntax invocation,
            out string source,
            out string destination)
        {
            source = destination = null!;

            GenericNameSyntax? genericName = invocation.Expression switch
            {
                // this.CreateMap<A, B>()
                MemberAccessExpressionSyntax memberAccess
                    => memberAccess.Name as GenericNameSyntax,

                // CreateMap<A, B>()
                GenericNameSyntax name
                    => name,

                _ => null
            };

            if (genericName == null)
                return false;

            if (!Configuration.CreateMapFunctionNames.Contains(
                    genericName.Identifier.Text,
                    StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            var args = genericName.TypeArgumentList.Arguments;

            if (args.Count >= 2)
            {
                source = args[0].ToString();
                destination = args[1].ToString();

                return true;
            }

            return false;
        }

        private static bool HasReverseMap(InvocationExpressionSyntax createMapInvocation)
		{
			SyntaxNode current = createMapInvocation.Parent!;

			while (current is MemberAccessExpressionSyntax or InvocationExpressionSyntax)
			{
				if (current is InvocationExpressionSyntax invocation &&
					invocation.Expression is MemberAccessExpressionSyntax member &&
					Configuration.ReverseMapFunctionNames.Contains(member.Name.Identifier.Text, StringComparer.OrdinalIgnoreCase))
				{
					return true;
				}

				current = current.Parent!;
			}

			return false;
		}

	}
}

