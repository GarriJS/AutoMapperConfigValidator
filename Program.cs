namespace AutoMapperConfigValidator
{
	public static class Program
	{
		public static void Main()
		{
			Console.WriteLine("Hello, welcome to the AutoMapper Configuration Validator.");
			var rootProfilePath = GetRootProfilePath();
			Console.WriteLine("Directory exists and was valid, compiling mapping profiles...");
			var autoMapperProfiles = CsFileFinderUtil.FindMatchingCsFiles(rootProfilePath, Configuration.GetMappingProfileMatchCriteria)?.ToArray();

			if (autoMapperProfiles == null ||
				autoMapperProfiles.Length == 0)
			{
				Console.WriteLine("No AutoMapper profile for that root folder were found.");

				return;
			}

			Console.WriteLine($"Would you like to see the list of {autoMapperProfiles.Length} AutoMapper profile(s) we found? Y/N");
			var viewProfilesResposne = Console.ReadLine();

			if (viewProfilesResposne?.ToLower() == "y")
			{
				foreach (var mapperProfile in autoMapperProfiles)
				{
					Console.WriteLine(mapperProfile);
				}
			}

			Console.WriteLine("Compiling AutoMapper configurations from AutoMapper profiles...");
			var autoMapperConfigs = CompileMappingConfigurationsUtil.ExtractCreateMapTypes(autoMapperProfiles!).ToArray();
			Console.WriteLine($"{autoMapperConfigs.Length} AutoMapper configurations were found.");
			var rootProjectPaths = GetRootProjectPaths();
			Console.WriteLine("Directories exists and are valid, compiling .cs files...");
			var projectCsFiles = CompileProjectCsFiles(rootProjectPaths)?.ToArray();

			if (projectCsFiles == null ||
				projectCsFiles.Length == 0)
			{
				Console.WriteLine("No .cs files for those root folders were found.");

				return;
			}

			Console.WriteLine($"{projectCsFiles.Length} .cs files were found, compiling mapping instances...");
			var autoMapperMaps = AutoMapperMapExtractorUtil.ExtractMapInfosFromFile(projectCsFiles!).ToArray();
			var missingAutoMaps = MissingMapperFinderUtil.FindMissingMappings(autoMapperConfigs, autoMapperMaps).ToArray();

			if (missingAutoMaps == null ||
				missingAutoMaps.Length == 0)
			{
				Console.WriteLine("No missing AutoMapper maps were found.");

				return;
			}

			var mapDictionary = missingAutoMaps.GroupBy(map => map.TypeOut)          
											   .ToDictionary(g => g.Key, g => g.ToArray());
			Console.WriteLine($"{mapDictionary.Count} AutoMapper mappings are possibly missing.");
			var reportPath = MissingMappingsReportUtil.WriteMissingMappingsReport(autoMapperProfiles, mapDictionary);
			Console.WriteLine($"Missing mappings report written to: {reportPath}");
		}

		private static string GetRootProfilePath()
		{
			string rootPath;

			do
			{
				Console.WriteLine("Please enter the path of the root folder you would like to use AutoMapper profiles for Validation.");
				rootPath = Console.ReadLine() ?? string.Empty;

				if (!IsValidRootPath(rootPath))
				{
					Console.WriteLine("Path was not valid or file did not exist.");
				}

			} while (!IsValidRootPath(rootPath));

			return rootPath;
		}

		private static string[] GetRootProjectPaths()
		{
			string[] rootPaths;
			bool isValid;

			do
			{
				Console.WriteLine("Please enter the path of the root folder you would like to validate for mapping.");
				Console.WriteLine("Use | to separate multiple root folder.");
				Console.WriteLine("This is an intensive operation, target few projects at once.");
				var rawInput = Console.ReadLine() ?? string.Empty;
				rootPaths = [.. rawInput.Split('|').Distinct()];

				if (rootPaths == null ||
					rootPaths.Length == 0)
				{
					isValid = false;

					continue;
				}
					
				isValid = true;

				foreach (var rootPath in rootPaths!)
				{
					if (!IsValidRootPath(rootPath))
					{
						Console.WriteLine($"Path was not valid or file did not exist: {rootPath}");
						isValid = false;

						break;
					}
				}
			} while (!isValid);

			return rootPaths!;
		}

		private static bool IsValidRootPath(string? pathString)
		{
			return pathString != null && pathString != string.Empty && Directory.Exists(pathString);
		}

		private static IEnumerable<string> CompileProjectCsFiles(string[] rootProjectPaths)
		{
			foreach (var rootProjectPath in rootProjectPaths)
			{
				var projectCsFiles = CsFileFinderUtil.FindMatchingCsFiles(rootProjectPath)?.ToArray();

				if (projectCsFiles == null ||
					projectCsFiles.Length == 0)
				{
					Console.WriteLine($"We found no matching .cs files for the root folder {rootProjectPath}.");

					continue;
				}

				foreach (var projectCsFile in projectCsFiles!)
				{
					yield return projectCsFile;
				}
			}
		}
	}
}