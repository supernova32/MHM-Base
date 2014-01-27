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

namespace JPA.Android
{
	class DrawerAdapter : BaseAdapter<MenuItem>
	{
		readonly IList<MenuItem> items = new List<MenuItem> ();
		readonly Activity _context;

		public DrawerAdapter (Activity context) {
			_context = context;
			var pub = new MenuItem { Title = "Home", Image = Resource.Drawable.ic_home };
			items.Add (pub);
			var comp = new MenuItem { Title = "Companies", Image = Resource.Drawable.ic_companies };
			items.Add (comp);		
		}

		public override View GetView (int position, View convertView, ViewGroup parent) {
			var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.NavigationItem, null);
			var item = items[position];
			view.FindViewById<TextView>(Resource.Id.title).Text = item.Title; 
			view.FindViewById<ImageView> (Resource.Id.menu_image).SetImageResource (item.Image);
			return view; 
		}

		public override int Count {
			get { return items.Count; } 
		}

		public override long GetItemId(int position) {
			return position; 
		}

		public override MenuItem this[int index] {
			get { return items[index]; } 
		}
	}

	public class MenuItem {
		public String Title { get; set; }
		public int Image { get; set; }	
	}
}

