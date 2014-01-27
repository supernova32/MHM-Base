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
using Android.Webkit;
using Android.Support.V4.App;

namespace JPA.Android
{
	[Activity (Theme = "@style/Theme.Customactionbartheme")]
	[MetaData (("android.support.PARENT_ACTIVITY"), Value = "jpa.android.MainActivity")]			
	public class WebActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState) {
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			base.OnCreate (savedInstanceState);
			var b = Intent.Extras;
			Title = b.GetString ("company_name");
			SetContentView (Resource.Layout.Web);
			var link = FindViewById<WebView>(Resource.Id.LocalWebView);
			link.SetWebViewClient (new WebViewClient());
			link.Settings.JavaScriptEnabled = true;
			link.LoadUrl (b.GetString("url"));
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case global::Android.Resource.Id.Home:
				NavUtils.NavigateUpFromSameTask (this);
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.options_menu, menu);
			menu.RemoveItem (Resource.Id.search);
			menu.RemoveItem (Resource.Id.action_refresh);
			menu.RemoveItem (Resource.Id.action_clear_history);
			return true;
		}	
	}
}

