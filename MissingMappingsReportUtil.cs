using AutoMapperConfigValidator.Models;

namespace AutoMapperConfigValidator
{
	public static class MissingMappingsReportUtil
	{
		public static string WriteMissingMappingsReport(string[] autoMapperProfiles, Dictionary<string, AutoMapperMap[]> mapDictionary)
		{
			var reportPath = Path.Combine(AppContext.BaseDirectory, Configuration.ReportFileName);
			using var writer = new StreamWriter(reportPath);

			writer.WriteLine("AutoMapper Missing Mappings Report");
			writer.WriteLine($"Generated: {DateTime.Now}");
			writer.WriteLine(new string('-', 50));
			writer.WriteLine();

			if (autoMapperProfiles != null &&
				autoMapperProfiles.Length > 0)
			{
				writer.WriteLine("AutoMapper Profiles Used:");
				writer.WriteLine(new string('-', 30));

				foreach (var profile in autoMapperProfiles)
				{
					writer.WriteLine($"  {profile}");
				}

				writer.WriteLine();
				writer.WriteLine(new string('-', 50));
				writer.WriteLine();
			}

			foreach (var kvp in mapDictionary.OrderBy(k => k.Key))
			{
				var destinationType = kvp.Key;
				var maps = kvp.Value;
				writer.WriteLine($"Destination Type: {destinationType}");
				writer.WriteLine(new string('-', 30));

				foreach (var map in maps)
				{
					writer.WriteLine($"  Source Variable Name: {map.SourceVariableName}");
					writer.WriteLine($"  File: {map.FilePath}");
					writer.WriteLine($"  Line: {map.LineNumber}");
					writer.WriteLine();
				}

				writer.WriteLine();
			}

			writer.WriteLine("End of report");
			writer.Flush();

			return reportPath;
		}
	}
}
