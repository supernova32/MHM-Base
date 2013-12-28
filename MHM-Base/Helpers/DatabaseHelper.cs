using SQLite;

namespace MHMBase
{
	public class DatabaseHelper
	{
		static DatabaseHelper currentInstance;
		readonly SQLiteConnection db;

		public static DatabaseHelper Instance {
			get {
				if (currentInstance == null)
					currentInstance = new DatabaseHelper ();
				return currentInstance;				
			}
		}

		DatabaseHelper () {
			string folder = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			db = new SQLiteConnection (System.IO.Path.Combine (folder, "mhm-jpa.db"));		
		}

		public SQLiteConnection Connection {
			get { return db; }		
		}
	}
}