namespace AutoMapperConfigValidator.Models
{
	public class AutoMapperMap
	{
		public required string TypeOut { get; set; }

		public required string SourceVariableName { get; set; }

		public required string FilePath { get; set; }

		public required int LineNumber { get; set; }
	}
}
