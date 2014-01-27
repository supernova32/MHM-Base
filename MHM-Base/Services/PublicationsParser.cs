using System;
using System.Collections.Generic; 
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using SQLite;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace MHMBase
{
	public class PublicationsParser
	{
		const string _baseUrl = "https://insomniware.com/publications.xml";
		readonly SQLiteConnection db = DatabaseHelper.Instance.Connection;

		public void UpdatePublications(Action<IList<Publication>> callback, Action<bool> error) {
			db.CreateTable<Publication>();
			var client = new WebClient ();
			client.DownloadStringCompleted += (sender, args) => {
				try {
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
				} catch (System.Reflection.TargetInvocationException) {
					Console.WriteLine ("TargetInvocationException");
					error (true);			
				}


				callback(Publications);
			};

			client.DownloadStringAsync (new Uri (_baseUrl));
			
		}

		public void LocalSearch (Action<IList<Publication>> callback, string query) {
			var publications = db.Query<Publication> ("SELECT * FROM publications WHERE FullDescription LIKE ? OR ShortDescription LIKE ? OR Title LIKE ?", "%"+query+"%", "%"+query+"%", "%"+query+"%");
			callback (publications);		
		}

		public void SendSearchParameters (Action<IList<Publication>> callback, string parameters, Action<bool> error) {
			var http = (HttpWebRequest)WebRequest.Create (new Uri ("http://192.168.10.149:3000/api/v1/publications"));
			http.Accept = "application/json";
			http.ContentType = "application/json";
			http.Method = "POST";

			var jObject = new JObject (new JProperty ("query", parameters));
			var encoding = new UTF8Encoding ();
			Byte[] bytes = encoding.GetBytes (jObject.ToString ());
			IList<Publication> publications = new List<Publication> ();
			Task.Factory.StartNew (() =>  {
				var newStream = http.GetRequestStream ();
				newStream.Write (bytes, 0, bytes.Length);
				newStream.Close ();
				var response = http.GetResponse ();
				var stream = response.GetResponseStream ();
				var sr = new StreamReader (stream);
				var content = sr.ReadToEnd ();
				var resultJSON = JObject.Parse (content);
				publications = resultJSON ["publications"].Select (item => new Publication {
					RemoteId = item ["id"].ToString (),
					Title = (string)item ["title"],
					Company = (string)item ["company"],
					FullDescription = (string)item ["full_description"],
					ShortDescription = (string)item ["short_description"],
					Link = (string)item ["link"]
				}).ToList ();
			}).ContinueWith (state =>  {
				Console.WriteLine ("State of exec: " + state.Exception);
				if (state.Status == TaskStatus.Faulted) {
					error (true);
				}
				else {
					callback (publications);
				}
			});
		}

		public Publication GetRemotePublication (int remoteId) {
			var http = (HttpWebRequest)WebRequest.Create(new Uri("http://192.168.2.1:3000/api/v1/publications"));
			http.Accept = "application/json";
			http.ContentType = "application/json";
			http.Method = "POST";

			var jObject = new JObject (new JProperty ("publication_id", remoteId));
			var encoding = new UTF8Encoding();
			Byte[] bytes = encoding.GetBytes(jObject.ToString());

			var newStream = http.GetRequestStream();
			newStream.Write(bytes, 0, bytes.Length);
			newStream.Close();

			var response = http.GetResponse();

			var stream = response.GetResponseStream();
			var sr = new StreamReader (stream);
			var content = sr.ReadToEnd();
			var item = JObject.Parse (content);
			var publication = new Publication {
				RemoteId = item["id"].ToString(),
				Title = (string)item["title"],
				Company = (string)item["company"],
				FullDescription = (string)item["full_description"],
				ShortDescription = (string)item["short_description"],
				Link = (string)item["link"]					
			};

			return publication;	
		}

		public IList<Publication> Publications {
			get {
				return db.Table<Publication>().OrderByDescending(p => p.Id).ToList();			
			}
		}
	}
}

