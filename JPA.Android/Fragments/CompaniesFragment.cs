using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Support.V4.View;
using Android.Widget;
using MHMBase;

namespace JPA.Android
{		
	public class CompaniesFragment : Fragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			Activity.SetTitle (Resource.String.companies);
			SetHasOptionsMenu (true);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var parser = new CompaniesParser ();
			var companies = parser.GetCompanies (DatabaseHelper.Instance.Connection);
			var layout = inflater.Inflate(Resource.Layout.CompaniesGrid, container, false);
			var companiesGrid = layout.FindViewById<GridView> (Resource.Id.Companies);
			companiesGrid.Adapter = new CompaniesAdapter (inflater, companies);
			companiesGrid.ItemClick += (sender, e) => {
				var comp = companies [e.Position];
				var publications = new PublicationsFragment (false, comp.Id);
				MainActivity.mDrawerToggle.DrawerIndicatorEnabled = false;
				FragmentManager.BeginTransaction ().Replace (Resource.Id.content_frame, publications).AddToBackStack (null).Commit ();
			};
			return layout;
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			menu.RemoveItem (Resource.Id.search);
			Console.Write ("Menu Creator called");
		}
	}
}

