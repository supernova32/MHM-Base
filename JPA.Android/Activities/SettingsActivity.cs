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
using Android.Preferences;

namespace JPA.Android
{
	[Activity (Label = "Settings")]			
	public class SettingsActivity : PreferenceActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			// Create your application here
		}
	}
}

