using System;
using MonoTouch.UIKit;
using SQLite;
using MHMBase;

namespace JPA.iOS_Normal
{
	public class SearchViewController : UITableViewController
	{
		readonly PublicationsParser _parser;
		readonly SQLiteConnection _db;
		readonly UISearchBar searchBar;

		public SearchViewController (SQLiteConnection db) : base (UITableViewStyle.Grouped) {
			_db = db;
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

		protected void RefineSearchItems() {
			if (searchBar.Text == "") {
				TableView.Source = null;
				TableView.ReloadData ();
				Console.WriteLine ("Cancelled");

			} else {
				if (PublicationsViewController.NetworkAvailable ()) {
					_parser.SendSearchParameters (publications => InvokeOnMainThread (() => {
						TableView.Source = new PublicationsViewSource (publications, this, _db);
						TableView.ReloadData ();
					}), searchBar.Text);
				} else {
					new UIAlertView ("Network".t(), "NetworkMessage".t(), null, "Ok", null).Show ();
				}			
			}			
		}

	}
}



