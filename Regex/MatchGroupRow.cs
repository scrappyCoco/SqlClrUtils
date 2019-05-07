using JetBrains.Annotations;

namespace Coding4fun.MsSql.Regex
{
	[PublicAPI]
	public class MatchGroupRow
	{
		public int MatchNumber { get; set; }
		public int GroupNumber { get; set; }
		public int Index { get; set; }
		public int Length { get; set; }
		public string Value { get; set; }
		public string GroupName { get; set; }
	}
}