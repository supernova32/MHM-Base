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
		LinearLayout LoadingView;

		public PublicationsFragment (bool remoteLoad = true, int companyId = 0) {
			_reload = remoteLoad;
			_companyId = companyId;		
		}

		public PublicationsFragment () {
			_reload = true;
			_companyId = 0;
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

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			if (_companyId != 0)
				menu.RemoveItem (Resource.Id.action_refresh);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case global::Android.Resource.Id.Home:
				Activity.SetTitle (Resource.String.companies);
				Activity.OnBackPressed ();
				return true;
			case Resource.Id.action_refresh:
				RefreshTable ();
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
//			if (_companyId == 0) {
//				layout = _inflater.Inflate (Resource.Layout.RefreshPubList, container, false);
//				var list = layout.FindViewById<PullToRefresharp.Android.Widget.ListView> (Resource.Id.Publications);
//				list.RefreshActivated += (sender, e) => RefreshTable (list);
//				publicationsList = list;
//			} else {
				layout = _inflater.Inflate (Resource.Layout.PublicationsList, container, false);
				publicationsList = layout.FindViewById<ListView> (Resource.Id.Publications);
				LoadingView = layout.FindViewById<LinearLayout> (Resource.Id.load_status);
//			}
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

		public void RefreshTable () { //PullToRefresharp.Android.Views.IPullToRefresharpView list = null
			var activity = Activity;
			publicationsList.Visibility = ViewStates.Gone;
			LoadingView.Visibility = ViewStates.Visible;
			if (cnHelper.NetworkAvailable ()) {
				parser.UpdatePublications (publications => activity.RunOnUiThread (() => {
					var adapter = new PublicationsListAdapter (_inflater, publications);
					publicationsList.Adapter = adapter;
					publicationsList.ItemClick += (sender, e) => {
						var pub = adapter.Publications [e.Position];
						var intent = new Intent (activity, typeof(PublicationActivity));
						intent.PutExtra ("pub_id", pub.Id);
						StartActivity (intent);
					};
					LoadingView.Visibility = ViewStates.Gone;
					publicationsList.Visibility = ViewStates.Visible;
				}), error => activity.RunOnUiThread (() => Toast.MakeText (activity, Resource.String.connection_error, ToastLength.Long).Show ()));
			} else {
				SetupTable (_companyId);
			}
//			if (list != null)
//				list.OnRefreshCompleted ();		
		}
	}
}

