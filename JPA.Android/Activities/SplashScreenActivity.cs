using System;
using Android.App;
using Android.OS;
using PerpetualEngine.Storage;
using MHMBase;
using Android.Widget;
using Android.Views;
using Android.Content.PM;

namespace JPA.Android
{
	[Activity (Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]			
	public class SplashScreenActivity : Activity
	{
		ProgressBar status;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.SplashView);
			status = FindViewById<ProgressBar> (Resource.Id.setup_bar);
			status.Visibility = ViewStates.Visible;
			SimpleStorage.SetContext(ApplicationContext);
			var storage = SimpleStorage.EditGroup("preferences");
			var value = storage.Get("companies_exist");
			var cnHelper = ConnectivityHelper.Instance (this);
			if (value == null) {
				if (cnHelper.NetworkAvailable ()) {
					UpdateCompanies (storage);
				} else {
					var builder = new AlertDialog.Builder (this);
					builder.SetMessage (Resource.String.network_needed).SetTitle (Resource.String.no_network);
					builder.SetPositiveButton (Resource.String.ok, delegate {
						Finish ();
					});
					var alert = builder.Create ();
					alert.Show ();
				}
			} else {
				if (cnHelper.NetworkAvailable ()) {
					UpdateCompanies (storage);
				} else {
					StartActivity (typeof(MainActivity));
				}
			}
		}

		void UpdateCompanies (SimpleStorage storage) {
			var cp = new CompaniesParser ();
			cp.PopulateCompaniesDB (success => RunOnUiThread (() => {
				if (success)
					storage.Put ("companies_exist", "True");
				StartActivity (typeof(MainActivity));
			}), state => RunOnUiThread (() => {
				var builder = new AlertDialog.Builder (this);
				builder.SetMessage (Resource.String.network_needed).SetTitle (Resource.String.no_network);
				builder.SetPositiveButton (Resource.String.ok, delegate {
					Finish ();
				});
				var alert = builder.Create ();
				alert.Show ();
			}));		
		}
	}
}

