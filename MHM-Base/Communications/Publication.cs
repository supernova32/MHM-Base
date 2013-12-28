using SQLite;
namespace MHMBase
{
	[Table ("publications")]
	public class Publication
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string RemoteId { get; set; }
		public string Title { get; set; }
		public string FullDescription { get; set; }
		public string Company { get; set; }
		[Unique]
		public string Link { get; set; }
		public string ShortDescription { get; set; }
		public bool Favorited { get; set; }
	}
}