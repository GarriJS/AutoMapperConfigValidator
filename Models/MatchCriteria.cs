namespace AutoMapperConfigValidator.Models
{
	public class MatchCriteria
	{
		public required string[] ContainCriteria { get; set; }


		public bool Matches(string item)
		{
			foreach (var criteria in this.ContainCriteria)
			{
				if (item.Contains(criteria))
				{ 
					return true;
				}
			}

			return false;
		}
	}
}
