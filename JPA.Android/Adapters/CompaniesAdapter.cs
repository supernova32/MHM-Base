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
	class CompaniesAdapter : BaseAdapter<Company>
	{
		readonly LayoutInflater _context;
		readonly IList<Company> _companies;

		public CompaniesAdapter (LayoutInflater context, IList<Company> companies) {
			_context = context;
			_companies = companies;		
		}

		public override View GetView (int position, View convertView, ViewGroup parent) {
			var view = convertView ?? _context.Inflate(Resource.Layout.Company, null);
			var company = _companies[position];
			var imgFile = new File (company.IconPath);
			Bitmap imgBitmap = BitmapFactory.DecodeFile(imgFile.AbsolutePath);
			view.FindViewById<TextView>(Resource.Id.Name).Text = company.FullName; 
			view.FindViewById<ImageView> (Resource.Id.company_image).SetImageBitmap (imgBitmap);
			return view; 
		}

		public override int Count {
			get { return _companies.Count; } 
		}

		public override long GetItemId(int position) {
			return position; 
		}

		public override Company this[int index] {
			get { return _companies[index]; } 
		}
	}
}

