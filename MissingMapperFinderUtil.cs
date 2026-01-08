using AutoMapperConfigValidator.Models;

namespace AutoMapperConfigValidator
{
	public static class MissingMapperFinderUtil
	{
		public static IEnumerable<AutoMapperMap> FindMissingMappings(AutoMapperConfig[] configs, AutoMapperMap[] maps)
		{
			foreach (var map in maps)
			{
				if (!MapHasMatchingConfig(configs, map))
				{ 
					yield return map;
				}
			}
		}

		private static bool MapHasMatchingConfig(AutoMapperConfig[] configs, AutoMapperMap map)
		{
			var matchingConfig = configs.FirstOrDefault(e => e.TypeIn == map.TypeOut);

			return matchingConfig != null;
		}
	}
}
