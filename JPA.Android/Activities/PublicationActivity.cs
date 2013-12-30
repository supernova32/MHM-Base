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
using Java.IO;
using Android.Graphics;
using Android.Support.V4.App;

namespace JPA.Android
{
	[Activity (Theme = "@android:style/Theme.Holo.Light")]
	[MetaData (("android.support.PARENT_ACTIVITY"), Value = "jpa.android.MainActivity")]			
	public class PublicationActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			SetContentView (Resource.Layout.PublicationView);
			ActionBar.SetDisplayHomeAsUpEnabled (true);
			base.OnCreate (savedInstanceState);
			var bundle = Intent.Extras;
			var id = bundle.GetInt ("pub_id");
			var db = DatabaseHelper.Instance.Connection;
			var pub = db.Get<Publication> (id);
			Title = pub.Title;
			var image = FindViewById<ImageView> (Resource.Id.company_image);
			var title = FindViewById<TextView> (Resource.Id.title);
			var description = FindViewById<TextView> (Resource.Id.description);
			var company = db.Table<Company> ().Where (c => c.Name.Equals(pub.Company)).First();
			var imgFile = new File (company.IconPath);
			Bitmap imgBitmap = BitmapFactory.DecodeFile(imgFile.AbsolutePath);
			image.SetImageBitmap (imgBitmap);
			title.Text = pub.Title;
			description.Text = pub.FullDescription;

			// Create your application here
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case global::Android.Resource.Id.Home:
				//OnBackPressed ();
				NavUtils.NavigateUpFromSameTask (this);
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}
	}
}

