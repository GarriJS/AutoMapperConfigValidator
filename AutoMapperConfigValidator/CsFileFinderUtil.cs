using AutoMapperConfigValidator.Models;

namespace AutoMapperConfigValidator
{
	public static class CsFileFinderUtil
	{
		public static IEnumerable<string> FindMatchingCsFiles(string rootPath, MatchCriteria? matchCriteria = null)
		{
			var stack = new Stack<string>();
			stack.Push(rootPath);

			while (stack.Count > 0)
			{
				var current = stack.Pop();

				string[] files;
				string[] directories;

				try
				{
					files = Directory.GetFiles(current, "*.cs");
					directories = Directory.GetDirectories(current);
				}
				catch (UnauthorizedAccessException)
				{
					continue;
				}
				catch (DirectoryNotFoundException)
				{
					continue;
				}

				foreach (var file in files)
				{
					if (false == matchCriteria?.Matches(file))
					{
						continue;
					}

					yield return file;
				}

				foreach (var dir in directories)
				{
					stack.Push(dir);
				}
			}
		}
	}
}
