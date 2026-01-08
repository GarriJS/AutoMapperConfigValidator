namespace AutoMapperConfigValidator.Models
{
	public class AutoMapperConfig
	{
		public required string TypeIn { get; set; }

		public required string TypeOut { get; set; }

		public required bool IsReverseMap { get; set; }
	}
}
