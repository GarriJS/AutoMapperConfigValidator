using AutoMapperConfigValidator.Models;

namespace AutoMapperConfigValidator
{
	public static class Configuration
	{
		private static readonly MatchCriteria mappingProfileMatchCriteria = new()
		{
			ContainCriteria = ["AutoMapperProfile"]
		};
		private static readonly string[] createMapFunctionNames = ["CreateMap"];
		private static readonly string[] reverseMapFunctionNames = ["ReverseMap"];
		private static readonly string[] mapFunctionNames = ["Map"];
        private static readonly string[] typeOutContainsMuteList = ["List<"];
		private static readonly string[] muteFileForMissingMappings = ["Repository"];

		public static MatchCriteria GetMappingProfileMatchCriteria { get => mappingProfileMatchCriteria; }
		public static string[] CreateMapFunctionNames { get => createMapFunctionNames; }
		public static string[] ReverseMapFunctionNames { get => reverseMapFunctionNames; }
		public static string[] MapFunctionNames { get => mapFunctionNames; }
        public static string ReportFileName { get => "MissingAutoMapperMappings.txt"; }
		public static string[] TypeOutContainsMuteList { get => typeOutContainsMuteList; }
		public static string[] MuteFileForMissingMappings { get => muteFileForMissingMappings; }
	}
}
