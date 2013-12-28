using System;
using Android.App;
using Android.OS;
using PerpetualEngine.Storage;
using AndroidHUD;
using MHMBase;

namespace JPA.Android
{
	[Activity (Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]			
	public class SplashScreenActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SimpleStorage.SetContext(ApplicationContext);
			AndHUD.Shared.Show(this, "Loading", -1,  MaskType.Clear);
			var storage = SimpleStorage.EditGroup("preferences");
			var value = storage.Get("companies_exist");
			var cnHelper = ConnectivityHelper.Instance (this);
			if (value == null) {
				if (cnHelper.NetworkAvailable ()) {
					UpdateCompanies (storage);
				} else {
					AndHUD.Shared.ShowError (this, "Network connection required", MaskType.Black, TimeSpan.FromSeconds (5));
				}
			} else {
				if (cnHelper.NetworkAvailable ()) {
					UpdateCompanies (storage);
				} else {
					AndHUD.Shared.Dismiss (this);
					StartActivity (typeof(MainActivity));
				}
			}
		}

		void UpdateCompanies (SimpleStorage storage) {
			var cp = new CompaniesParser ();
			cp.PopulateCompaniesDB (success => RunOnUiThread (() => {
				if (success)
					storage.Put ("companies_exist", "True");
				AndHUD.Shared.Dismiss (this);
				StartActivity (typeof(MainActivity));
			}), DatabaseHelper.Instance.Connection);		
		}
	}
}

