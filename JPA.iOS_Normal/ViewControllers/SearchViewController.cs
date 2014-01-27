using System;
using MonoTouch.UIKit;
using MHMBase;

namespace JPA.iOS_Normal
{
	public class SearchViewController : UITableViewController
	{
		readonly PublicationsParser _parser;
		readonly UISearchBar searchBar;

		public SearchViewController () : base (UITableViewStyle.Grouped) {
			_parser = new PublicationsParser ();
			searchBar = new UISearchBar ();
		}
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			searchBar.SizeToFit ();
			searchBar.AutocorrectionType = UITextAutocorrectionType.No;
			searchBar.AutocapitalizationType = UITextAutocapitalizationType.None;
			Title = "Search".t ();
			TableView.TableHeaderView = searchBar;
			searchBar.SearchButtonClicked += (s, e) => searchBar.ResignFirstResponder ();
			searchBar.TextChanged += (s, e) => RefineSearchItems ();
			TableView.BackgroundColor = UIColor.FromWhiteAlpha(0.95f, 1.0f);

			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}

		protected void RefineSearchItems() {
			if (searchBar.Text == "") {
				TableView.Source = null;
				TableView.ReloadData ();
				Console.WriteLine ("Cancelled");

			} else {
				if (PublicationsViewController.NetworkAvailable ()) {
					_parser.SendSearchParameters (publications => InvokeOnMainThread (() => {
						TableView.Source = new PublicationsViewSource (publications, NavigationController);
						TableView.ReloadData ();
					}), searchBar.Text, state => InvokeOnMainThread (() => { 
						var alert = new UIAlertView ("Error".t(), "ErrorMessage".t(), null, "Ok", null);
						//alert.Clicked += (sender, e) => UIApplication.SharedApplication.PerformSelector(new Selector("terminateWithSuccess"), null, 0f);
						alert.Show ();
					}));
				} else {
					_parser.LocalSearch (publications => InvokeOnMainThread (() => {
						TableView.Source = new PublicationsViewSource (publications, NavigationController);
						TableView.ReloadData ();
					}), searchBar.Text);
				}			
			}			
		}
	}
}



