using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MHMBase;
using Android.Content.PM;

namespace JPA.Android
{
	[Activity (LaunchMode = LaunchMode.SingleTop)]		
	public class SearchActivity : Activity
	{
		ConnectivityHelper cnHelper;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			HandleIntent (Intent);
			base.OnCreate (savedInstanceState);
			cnHelper = ConnectivityHelper.Instance (this);
			SetContentView (Resource.Layout.PublicationsList);
			SetTitle (Resource.String.search_results);


			// Create your application here
		}

		protected override void OnNewIntent (Intent intent)
		{
//			base.OnNewIntent (intent);
			HandleIntent (intent);
		}


		void HandleIntent (Intent intent) {
			if (Intent.ActionSearch.Equals (intent.Action)) {
				var query = intent.GetStringExtra (SearchManager.Query);
				if (cnHelper.NetworkAvailable ()) {
					var parser = new PublicationsParser ();
					parser.SendSearchParameters (publications => RunOnUiThread(() => {
						var publicationsList = FindViewById<ListView> (Resource.Id.Publications);
						publicationsList.Adapter = new PublicationsListAdapter (this.LayoutInflater, publications);
						publicationsList.ItemClick += (sender, e) => {
							var pub = publications [e.Position];
							var myIntent = new Intent(this, typeof(PublicationActivity));
							myIntent.PutExtra("remote_id", pub.RemoteId);
							StartActivity(myIntent);
						};
					}), query);				
				}			
			}		
		}
	}
}

