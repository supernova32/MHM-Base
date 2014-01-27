using System;
using Android.App;
using Android.Content.Res;
using Android.Views;
using Android.Support.V4.Widget;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Widget;
using Android.Content;
using Android.Content.PM;
using MHMBase;
using Android.Provider;
using Android.Gms.Common;
using Android.Gms.Gcm;
using PerpetualEngine.Storage;
using System.Threading.Tasks;

namespace JPA.Android
{
	[Activity (Label = "Latest Jobs", Theme = "@style/Theme.Customactionbartheme", LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize )] //Theme = "@android:style/Theme.Holo.Light"
	[IntentFilter (new [] { Intent.ActionSearch })]
	[MetaData (("android.app.searchable"), Resource = "@xml/searchable")]
	public class MainActivity : Activity
	{
		ConnectivityHelper cnHelper;
		DrawerLayout mDrawerLayout;
		ListView mDrawerList;
		PublicationsFragment pubs;
		LinearLayout LoadingView;
		SearchView searchView;
		public static ActionBarDrawerToggle DrawerToggle;
		int mTitle = Resource.String.main_title;

		const string SenderID = "499607806970";
		const string PropertyRegID = "registration_id";
		string RegID;
		GoogleCloudMessaging GCM;
		SimpleStorage storage;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			RequestWindowFeature (WindowFeatures.IndeterminateProgress);
			SetContentView (Resource.Layout.Navigation);

			if (HasPlayServices) {
				GCM = GoogleCloudMessaging.GetInstance (this);
				RegID = RegistrationId;
				if (String.IsNullOrEmpty (RegID)) {
					RegisterInBackground ();				
				} else {
					Console.WriteLine ("Seems like we have a RegID");
				}
			}


			cnHelper = ConnectivityHelper.Instance (this);
			mDrawerLayout = FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			mDrawerList = FindViewById<ListView> (Resource.Id.left_drawer);
			mDrawerList.Adapter = new DrawerAdapter (this);
			mDrawerList.ItemClick += (sender, e) => {
				switch (e.Position) {
				case 0:
					var publications = new PublicationsFragment (false);
					FragmentManager.BeginTransaction ().Replace (Resource.Id.content_frame, publications).Commit ();
					SetTitle (Resource.String.main_title);
					mDrawerLayout.CloseDrawer (mDrawerList);
					break;
				case 1:
					var companies = new CompaniesFragment ();
					FragmentManager.BeginTransaction ().Replace (Resource.Id.content_frame, companies).Commit ();
					SetTitle (Resource.String.companies);
					mDrawerLayout.CloseDrawer (mDrawerList);
					break;
				} 
			};

			mDrawerLayout.SetDrawerShadow (Resource.Drawable.drawer_shadow, GravityCompat.Start);

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			DrawerToggle = new ActionBarDrawerToggle (this, mDrawerLayout, Resource.Drawable.ic_navigation_drawer, Resource.String.drawer_open, Resource.String.drawer_close);
			mDrawerLayout.SetDrawerListener (DrawerToggle);

			mDrawerLayout.DrawerOpened += delegate {
				ActionBar.SetTitle (Resource.String.menu);
				//mDrawerLayout.
				InvalidateOptionsMenu ();
			};
			mDrawerLayout.DrawerClosed += delegate {
				SetTitle (mTitle);
				InvalidateOptionsMenu ();
			};

			pubs = new PublicationsFragment ();
			FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, pubs).Commit();
			mDrawerList.SetItemChecked (0, true);
			HandleIntent (Intent);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			var fragment = FragmentManager.FindFragmentById (Resource.Id.content_frame);
			Console.WriteLine (fragment);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case Resource.Id.action_clear_history:
				var suggestions = new SearchRecentSuggestions (this, SuggestionProvider.Authority, SuggestionProvider.Mode);
				var builder = new AlertDialog.Builder (this);
				builder.SetMessage (Resource.String.confirm_clear).SetTitle (Resource.String.action_clear_history);
				builder.SetPositiveButton (Resource.String.yes, delegate {
					suggestions.ClearHistory ();
					Toast.MakeText (this, Resource.String.clear_successful, ToastLength.Short).Show ();
				}); 
				builder.SetNegativeButton (Resource.String.cancel, CancelClicked);
				var alert = builder.Create ();
				alert.Show ();
				return true;
			case Resource.Id.action_settings:
				var reg = storage.Get (PropertyRegID);
				Toast.MakeText (this, reg, ToastLength.Long).Show ();
				Console.WriteLine ("From storage: " + reg);
				Console.WriteLine ("From local: " + RegID);
				return true;
			case Resource.Id.action_refresh:
				pubs.RefreshTable ();
				return true;
			}
			return DrawerToggle.OnOptionsItemSelected (item) || base.OnOptionsItemSelected (item);
		}

		static void CancelClicked (object sender, DialogClickEventArgs e) {
			((AlertDialog)sender).Cancel ();
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.options_menu, menu);
			var searchManager =	(SearchManager) GetSystemService (Context.SearchService);
			searchView = (SearchView) menu.FindItem (Resource.Id.search).ActionView;
			//var searchItem = menu.FindItem (Resource.Id.search);

			int id = searchView.Context.Resources.GetIdentifier ("android:id/search_src_text", null, null);
			var searchText = (AutoCompleteTextView) searchView.FindViewById (id);
			searchText.SetTextColor (Resources.GetColor (Resource.Color.white));
			searchText.SetHintTextColor (Resources.GetColor (Resource.Color.white));
			searchView.SetSearchableInfo (searchManager.GetSearchableInfo (ComponentName));
			searchView.Close += (sender, e) => {
				LoadingView.Visibility = ViewStates.Gone;
				FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, pubs).Commit();			
			};
			return true;
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			DrawerToggle.SyncState();
		}

		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			DrawerToggle.DrawerIndicatorEnabled = true;
		}

		public override void OnConfigurationChanged (Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			DrawerToggle.OnConfigurationChanged(newConfig);
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
				var suggestions = new SearchRecentSuggestions (this, SuggestionProvider.Authority, SuggestionProvider.Mode);
				suggestions.SaveRecentQuery (query, null);
				var parser = new PublicationsParser ();
				SetTitle (Resource.String.search_results);
				mDrawerList.SetItemChecked (0, false);
				var publicationsList = FindViewById<ListView> (Resource.Id.Publications);
				SetProgressBarIndeterminateVisibility (true);
				LoadingView = FindViewById<LinearLayout> (Resource.Id.load_status);
				publicationsList.Visibility = ViewStates.Gone;
				LoadingView.Visibility = ViewStates.Visible;
				if (cnHelper.NetworkAvailable ()) {
					parser.SendSearchParameters (publications => RunOnUiThread (() => {
						var adapter = new PublicationsListAdapter (this.LayoutInflater, publications);
						publicationsList.Adapter = adapter;
						publicationsList.ItemClick += (sender, e) => {
							var pub = adapter.Publications [e.Position];
							var myIntent = new Intent (this, typeof(PublicationActivity));
							myIntent.PutExtra ("remote_id", pub.RemoteId);
							StartActivity (myIntent);
						};
						SetProgressBarIndeterminateVisibility (false);
						LoadingView.Visibility = ViewStates.Gone;
						publicationsList.Visibility = ViewStates.Visible;
					}), query, state => RunOnUiThread (() => {
						var builder = new AlertDialog.Builder (this);
						builder.SetMessage (Resource.String.connection_error).SetTitle (Resource.String.error);
						builder.SetPositiveButton (Resource.String.ok, delegate {
							LoadingView.Visibility = ViewStates.Gone;
							publicationsList.Visibility = ViewStates.Visible;
							FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, pubs).Commit();
							SetProgressBarIndeterminateVisibility (false);
						});
						var alert = builder.Create ();
						alert.Show (); 
						//Toast.MakeText (this, Resource.String.connection_error, ToastLength.Short).Show (); 
					}));				
				} else {
					parser.LocalSearch (publications => RunOnUiThread (() => {
						var adapter = ((PublicationsListAdapter) publicationsList.Adapter);
						adapter.Publications = publications;
						adapter.NotifyDataSetChanged ();
						publicationsList.ItemClick += (sender, e) => {
							var pub = adapter.Publications [e.Position];
							var myIntent = new Intent (this, typeof(PublicationActivity));
							myIntent.PutExtra ("pub_id", pub.Id);
							StartActivity (myIntent);
						};
//						FrameView.Visibility = ViewStates.Visible;
						LoadingView.Visibility = ViewStates.Gone;
					}) , query);
				}			
			}		
		}

		bool HasPlayServices {
			get {
				int resultCode = GooglePlayServicesUtil.IsGooglePlayServicesAvailable (this);
				if (resultCode != ConnectionResult.Success) {
					if (GooglePlayServicesUtil.IsUserRecoverableError (resultCode)) {
						GooglePlayServicesUtil.GetErrorDialog (resultCode, this, 9000).Show ();
					} else {
						Console.WriteLine ("Unsupported device");				
					}
					return false;			
				}
				return true;
			}		
		}

		string RegistrationId {
			get {
				storage = SimpleStorage.EditGroup("preferences");
				var regId = storage.Get (PropertyRegID);
				Console.WriteLine ("Fetched ID: " + regId);
				return regId ?? "";
				// TODO Check if app was updated; if so, it must clear the registration ID
				// since the existing regID is not guaranteed to work with the new
				// app version.
			}

		}

		void RegisterInBackground ()
		{
			Task.Factory.StartNew (() => {
				Console.WriteLine ("Starting Background Thread");
				try {
					if (GCM == null)
						GCM = GoogleCloudMessaging.GetInstance (ApplicationContext);
					RegID = GCM.Register (SenderID);
					Console.WriteLine ("From Background task: " + RegID);
					storage.Put (PropertyRegID, RegID);

					// TODO You should send the registration ID to your server over HTTP, so it
					// can use GCM/HTTP or CCS to send messages to your app.
					// sendRegistrationIdToBackend();

				} catch (Java.IO.IOException) {
					storage.Put ("failed_registration", true);
				}			
			
			}).ContinueWith (task => RunOnUiThread (() => Toast.MakeText (this, "Background Task Finished", ToastLength.Long)));
		}
	}
}


