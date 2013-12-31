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

namespace JPA.Android
{
	class PublicationsListAdapter : BaseAdapter<Publication>
	{
		readonly LayoutInflater _context; 
		public IList<Publication> Publications { get; set; }

		public PublicationsListAdapter (LayoutInflater context, IList<Publication> publications) {
			_context = context;
			Publications = publications;		
		}

		public override View GetView(int position, View convertView, ViewGroup parent) {
			var view = convertView ?? _context.Inflate(Resource.Layout.Publication, null);
			var pub = Publications[position];
			var db = DatabaseHelper.Instance.Connection;
			var company = db.Table<Company> ().Where (c => c.Name.Equals(pub.Company)).First();
			var imgFile = new File (company.IconPath);
			Bitmap imgBitmap = BitmapFactory.DecodeFile(imgFile.AbsolutePath);
			view.FindViewById<TextView>(Resource.Id.Title).Text = pub.Title; 
			view.FindViewById<TextView> (Resource.Id.Description).Text = pub.ShortDescription;
			view.FindViewById<ImageView> (Resource.Id.company_image).SetImageBitmap (imgBitmap);
			return view; 
		}

		public override int Count {
			get { return Publications.Count; } 
		}

		public override long GetItemId(int position) {
			return position; 
		}

		public override Publication this[int index] {
			get { return Publications[index]; } 
		}
	}
}

