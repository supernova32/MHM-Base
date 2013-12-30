using System;
using Android.App;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using Android.Support.V4.Widget;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Widget;
using Android.Content;
using Android.Content.PM;
using MHMBase;

namespace JPA.Android
{
	[Activity (Label = "Latest Jobs", Theme = "@android:style/Theme.Holo.Light", LaunchMode = LaunchMode.SingleTop)]
	[IntentFilter (new [] {Intent.ActionSearch})]
	[MetaData (("android.app.searchable"), Resource = "@xml/searchable")]
	public class MainActivity : Activity
	{
		ConnectivityHelper cnHelper;
		DrawerLayout mDrawerLayout;
		ListView mDrawerList;
		public static ActionBarDrawerToggle mDrawerToggle;
		int mTitle = Resource.String.main_title;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Navigation);
			cnHelper = ConnectivityHelper.Instance (this);
			mDrawerLayout = FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			mDrawerList = FindViewById<ListView> (Resource.Id.left_drawer);
			mDrawerList.Adapter = new DrawerAdapter (this);
			mDrawerList.ItemClick += (sender, e) => {
				switch (e.Position) {
				case 1:
					var companies = new CompaniesFragment ();
					FragmentManager.BeginTransaction ().Replace (Resource.Id.content_frame, companies).Commit ();
					SetTitle (Resource.String.companies);
					mDrawerLayout.CloseDrawer (mDrawerList);
					break;
				case 0:
					var publications = new PublicationsFragment (false);
					FragmentManager.BeginTransaction ().Replace (Resource.Id.content_frame, publications).Commit ();
					SetTitle (Resource.String.main_title);
					mDrawerLayout.CloseDrawer (mDrawerList);
					break;
				} 
			};

			mDrawerLayout.SetDrawerShadow (Resource.Drawable.drawer_shadow, GravityCompat.Start);

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			mDrawerToggle = new ActionBarDrawerToggle (this, mDrawerLayout, Resource.Drawable.ic_drawer, Resource.String.drawer_open, Resource.String.drawer_close);
			mDrawerLayout.SetDrawerListener (mDrawerToggle);

			mDrawerLayout.DrawerOpened += delegate {
				ActionBar.SetTitle (Resource.String.menu);

				InvalidateOptionsMenu ();
			};
			mDrawerLayout.DrawerClosed += delegate {
				SetTitle (mTitle);
				InvalidateOptionsMenu ();
			};

			var pubs = new PublicationsFragment ();
			FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, pubs).Commit();
			HandleIntent (Intent);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			return mDrawerToggle.OnOptionsItemSelected (item) || base.OnOptionsItemSelected (item);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.options_menu, menu);

			var searchManager =	(SearchManager) GetSystemService (Context.SearchService);
			var searchView = (SearchView) menu.FindItem (Resource.Id.search).ActionView;
			searchView.SetSearchableInfo (searchManager.GetSearchableInfo (ComponentName));

			return true;
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			mDrawerToggle.SyncState();
		}

		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			mDrawerToggle.DrawerIndicatorEnabled = true;
		}

		public override void OnConfigurationChanged (Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			mDrawerToggle.OnConfigurationChanged(newConfig);
		}

		public override void SetTitle (int titleId) {
			mTitle = titleId;
			ActionBar.SetTitle (mTitle);		
		}

		protected override void OnNewIntent (Intent intent)
		{
			HandleIntent (intent);
		}

		void HandleIntent (Intent intent) {
			if (Intent.ActionSearch.Equals (intent.Action)) {
				var query = intent.GetStringExtra (SearchManager.Query);
				var parser = new PublicationsParser ();
				SetTitle (Resource.String.search_results);
				mDrawerList.SetItemChecked (0, false);
				var publicationsList = FindViewById<ListView> (Resource.Id.Publications);
				if (cnHelper.NetworkAvailable ()) {
					parser.SendSearchParameters (publications => RunOnUiThread (() => {
						publicationsList.Adapter = new PublicationsListAdapter (this.LayoutInflater, publications);
						publicationsList.ItemClick += (sender, e) => {
							var pub = publications [e.Position];
							var myIntent = new Intent (this, typeof(PublicationActivity));
							myIntent.PutExtra ("remote_id", pub.RemoteId);
							StartActivity (myIntent);
						};
					}), query);				
				} else {
					parser.LocalSearch (publications => RunOnUiThread (() => {
						//publicationsList.Adapter.Dispose ();
						//publicationsList.Adapter = new PublicationsListAdapter (this.LayoutInflater, publications);
						var adapter = ((PublicationsListAdapter) publicationsList.Adapter);
						adapter.Publications = publications;
						adapter.NotifyDataSetChanged ();
						publicationsList.ItemClick += (sender, e) => {
							var pub = adapter.Publications [e.Position];
							var myIntent = new Intent (this, typeof(PublicationActivity));
							myIntent.PutExtra ("pub_id", pub.Id);
							StartActivity (myIntent);
						};
					}) , query);
				}			
			}		
		}
	}
}


