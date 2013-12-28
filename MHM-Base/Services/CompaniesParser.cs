using System;
using System.Collections.Generic; 
using System.Linq;
using System.Net;
using System.Xml.Linq;
using SQLite;
using System.IO;

namespace MHMBase
{
	public class CompaniesParser
	{
		const string _baseUrl = "https://insomniware.com/companies.xml";
		public bool PopulateCompaniesDB(Action<bool> callback, SQLiteConnection db) {
			db.CreateTable<Company>();
			var client = new WebClient ();
			var success = false;
			client.DownloadStringCompleted += (sender, args) => {
				var companies = XDocument
					.Parse(args.Result)
					.Descendants("company").Select(item => new Company {
						Name = item.Element("name").Value,
						FullName = item.Element("full_name").Value,
						IconPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), item.Element("name").Value+".png"),
						IconUrl = item.Element("icon").Value
					}).ToList();

				foreach (var c in companies) {
					try {
						db.Insert (c);
						var webClient = new WebClient();
						var url = new Uri (c.IconUrl);
						var image_bytes = webClient.DownloadData (url);
						string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);	
						string localPath = Path.Combine (documentsPath, c.Name+".png");
						Console.WriteLine("localPath:"+localPath);
						File.WriteAllBytes (localPath, image_bytes);		
					} catch (SQLiteException) {
						Console.WriteLine ("CompaniesParser: Duplicate detected");					
					}
				}
				success = true;
				callback(success);

			};
			client.DownloadStringAsync (new Uri (_baseUrl));
			return success;
		}

		public IList<Company> GetCompanies(SQLiteConnection db) {
			var companies = db.Table<Company>().OrderBy(c => c.Name).ToList();
			return companies;
		}
	}
}

