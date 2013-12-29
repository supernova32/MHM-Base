using System;
using System.Collections.Generic; 
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using SQLite;
using System.Text;
using System.IO;

namespace MHMBase
{
	public class PublicationsParser
	{
		const string _baseUrl = "https://insomniware.com/publications.xml";
		readonly SQLiteConnection db = DatabaseHelper.Instance.Connection;

		public void UpdatePublications(Action<IList<Publication>> callback) {
			db.CreateTable<Publication>();
			var client = new WebClient ();
			client.DownloadStringCompleted += (sender, args) => {
				var pubs = XDocument
					.Parse(args.Result)
					.Descendants("publication").Select(item => new Publication {
						RemoteId = item.Element("id").Value,
						Title = item.Element("title").Value,
						Company = item.Element("company").Value,
						FullDescription = item.Element("full_description").Value,
						ShortDescription = item.Element("short_description").Value,
						Link = item.Element("link").Value
					}).ToList();

				foreach (var p in pubs) {
					try {
						db.Insert (p);
					} catch (SQLiteException){
						Console.WriteLine ("PublicationParser: Duplicate detected");
					}				
				}
				callback(Publications);
			};

			client.DownloadStringAsync (new Uri (_baseUrl));
		}

		public void LocalSearch (Action<IList<Publication>> callback, string query) {
			var publications = db.Query<Publication> ("SELECT * FROM publications WHERE FullDescription LIKE ? OR ShortDescription LIKE ? OR Title LIKE ?", "%"+query+"%", "%"+query+"%", "%"+query+"%");
			callback (publications);		
		}

		public void SendSearchParameters (Action<IList<Publication>> callback, string parameters) {
			var http = (HttpWebRequest)WebRequest.Create(new Uri("http://192.168.0.104:3000/api/v1/publications"));
			http.Accept = "application/json";
			http.ContentType = "application/json";
			http.Method = "POST";

			var jObject = new JObject (new JProperty ("query", parameters));
			var encoding = new UTF8Encoding();
			Byte[] bytes = encoding.GetBytes(jObject.ToString());

			var newStream = http.GetRequestStream();
			newStream.Write(bytes, 0, bytes.Length);
			newStream.Close();

			var response = http.GetResponse();

			var stream = response.GetResponseStream();
			var sr = new StreamReader (stream);
			var content = sr.ReadToEnd();
			var resultJSON = JObject.Parse (content);
			IList<Publication> publications = resultJSON["publications"].Select(item => new Publication	{
				RemoteId = item["id"].ToString(),
				Title = (string)item["title"],
				Company = (string)item["company"],
				FullDescription = (string)item["full_description"],
				ShortDescription = (string)item["short_description"],
				Link = (string)item["link"]					
			}).ToList();
			callback(publications);


//			var client = new WebClient();
//			client.Headers.Add("Content-Type", "application/json");
//
//			var json = @"{query:"+parameters+"}"; 

//			client.UploadDataCompleted += (sender, e) => {
//				var resultJSON = JObject.Parse (Encoding.UTF8.GetString (e.Result));
//				IList<Publication> publications = resultJSON["publications"].Select(item => new Publication	{
//					RemoteId = (string)item["id"],
//					Title = (string)item["title"],
//					Company = (string)item["company"],
//					FullDescription = (string)item["full_description"],
//					ShortDescription = (string)item["short_description"],
//					Link = (string)item["link"]					
//				}).ToList();
//				callback(publications);
//			};
//			client.UploadDataAsync (new Uri ("https://insomniware.com/"), JObject.Parse(json));		
		}

//		public Publication GetRemotePublication (int remoteId) {
//			var client = new WebClient();
//			client.Headers.Add("Content-Type", "application/json");
//		
//		}

		public IList<Publication> Publications {
			get {
				return db.Table<Publication>().ToList();			
			}
		}
	}
}

