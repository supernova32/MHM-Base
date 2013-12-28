using SQLite;

namespace MHMBase
{
	[Table("companies")]
	public class Company
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		[MaxLength(20), Unique]
		public string Name { get; set; }
		public string FullName { get; set; }
		public string IconPath { get; set; }
		public string IconUrl { get; set; }

	}
}

