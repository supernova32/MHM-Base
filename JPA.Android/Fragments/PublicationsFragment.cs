using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using MHMBase;

namespace JPA.Android
{
	public class PublicationsFragment : Fragment
	{
		readonly bool _reload;
		readonly int _companyId;
		View layout;
		PublicationsParser parser;
		DatabaseHelper dbHelper;
		ConnectivityHelper cnHelper;
		LayoutInflater _inflater;
		ListView publicationsList;

		public PublicationsFragment (bool remoteLoad = true, int companyId = 0) {
			_reload = remoteLoad;
			_companyId = companyId;		
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			if (_companyId == 0) {
				Activity.SetTitle (Resource.String.main_title);
			} else {
				Activity.SetTitle (Resource.String.by_company);
				SetHasOptionsMenu(true);
				Activity.ActionBar.SetDisplayHomeAsUpEnabled(true);			
			}
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case global::Android.Resource.Id.Home:
				Activity.SetTitle (Resource.String.companies);
				Activity.OnBackPressed ();
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			parser = new PublicationsParser ();
			dbHelper = DatabaseHelper.Instance;
			_inflater = inflater;
			cnHelper = ConnectivityHelper.Instance (Activity);
			if (_companyId == 0) {
				layout = _inflater.Inflate (Resource.Layout.RefreshPubList, container, false);
				var list = layout.FindViewById<PullToRefresharp.Android.Widget.ListView> (Resource.Id.Publications);
				list.RefreshActivated += (sender, e) => RefreshTable (list);
			} else {
				layout = _inflater.Inflate (Resource.Layout.PublicationsList, container, false);
			}
			publicationsList = layout.FindViewById<ListView> (Resource.Id.Publications);
			if (_reload) {
				RefreshTable ();	
			} else {
				SetupTable (_companyId);	
			}

			return layout;
		}

		public void SetupTable (int companyId) {
			IList<Publication> publications;

			if (companyId == 0) {
				publications = parser.Publications;
			} else {
				var company = dbHelper.Connection.Get<Company> (companyId);
				publications = dbHelper.Connection.Table<Publication> ().Where (p => p.Company.Equals (company.Name)).ToList ();
			}

			var adapter = new PublicationsListAdapter (_inflater, publications); 
			publicationsList.Adapter = adapter;
			publicationsList.ItemClick += (sender, e) => {
				var pub = adapter.Publications [e.Position];
				var intent = new Intent (Activity, typeof(PublicationActivity));
				intent.PutExtra ("pub_id", pub.Id);
				StartActivity (intent);
			};
		}

		public void RefreshTable (PullToRefresharp.Android.Widget.ListView list = null) {
			if (cnHelper.NetworkAvailable ()) {
				AndHUD.Shared.Show(Activity, "Downloading Jobs", -1, MaskType.Clear);
				parser.UpdatePublications (publications => Activity.RunOnUiThread (() => {
					publicationsList.Adapter = new PublicationsListAdapter (_inflater, publications);
					publicationsList.ItemClick += (sender, e) => {
						var pub = publications [e.Position];
						var intent = new Intent(Activity, typeof(PublicationActivity));
						intent.PutExtra("pub_id", pub.Id);
						StartActivity(intent);
					};
					AndHUD.Shared.Dismiss (Activity);
				}));
			} else {
				SetupTable (_companyId);
			}
			if (list != null)
				list.OnRefreshCompleted ();		
		}
	}
}

