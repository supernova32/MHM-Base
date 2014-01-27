using System.Drawing;
using MonoTouch.UIKit;
using MHMBase;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace JPA.iOS_Normal
{
	public class PublicationsViewController : UITableViewController
	{
		SplashScreen splash;
		bool showingSplashEarlier = false;
		bool _fromMenu = false;
		readonly PublicationsParser _parser;
		int _companyId;

		public PublicationsViewController (bool skipSplash, bool fromMenu = false, int companyId = 0) : base (UITableViewStyle.Plain)
		{
			_parser = new PublicationsParser ();
			showingSplashEarlier = skipSplash;
			_fromMenu = fromMenu;
			_companyId = companyId;
		}

		#region Modal view controller methods
		public void ShowModalVC(UIViewController modalViewController, bool animated = true)
		{
			PresentViewController(modalViewController, animated, null);
		}

		public void HideModalVC(bool animated = true)
		{
			DismissViewController(animated, null);
		}
		#endregion

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (!showingSplashEarlier) {
				showingSplashEarlier = true;
				splash = new SplashScreen ();
				splash.OnReady += () => {
					HideModalVC ();
					LoadTable (false);
				};
				ShowModalVC (splash, false);
			} else if (_companyId != 0) {
				FilterByCompany (_companyId);
			} else {
				LoadTable (showingSplashEarlier);
			}
		}

		public void LoadTable(bool skipRefresh) {
			if (!skipRefresh) {
				var loading = new UIAlertView ("DLoad".t (), "Wait".t (), null, null, null);
				loading.Show ();
				var indicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.WhiteLarge);
				indicator.Center = new PointF (loading.Bounds.Width / 2, loading.Bounds.Size.Height - 40);
				indicator.StartAnimating (); 
				loading.AddSubview (indicator);
				RefreshTable (loading);
			} else if (_fromMenu) {
				var publications = _parser.Publications;
				TableView.Source = new PublicationsViewSource (publications, NavigationController);
				TableView.ReloadData ();
			} else {
				RefreshTable ();			
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.BackgroundColor = UIColor.FromWhiteAlpha(0.95f, 1.0f);
			if (_companyId == 0) {
				Title = "LatestJobTitle".t ();
				RefreshControl = new UIRefreshControl ();
				RefreshControl.ValueChanged += (sender, e) => RefreshTable ();
			} else {
				Title = "ByCompany".t ();			
			}
		}

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}

		public static bool NetworkAvailable() {
			NetworkStatus status = Reachability.InternetConnectionStatus();
			return status != NetworkStatus.NotReachable;
		}

		void FilterByCompany (int companyId)
		{
			var dbHelper = DatabaseHelper.Instance;
			var company = dbHelper.Connection.Get<Company> (companyId);
			var publications = dbHelper.Connection.Table<Publication> ().Where (p => p.Company.Equals (company.Name)).ToList ();
			TableView.Source = new PublicationsViewSource (publications, NavigationController);
			TableView.ReloadData ();
		}

		public void RefreshTable(UIAlertView loading = null) {
			if (NetworkAvailable ()) {
				_parser.UpdatePublications(publications => InvokeOnMainThread (() => {
					TableView.Source = new PublicationsViewSource (publications, NavigationController);
					TableView.ReloadData ();
					if (loading != null)
						loading.DismissWithClickedButtonIndex (0, true);
				}), state => InvokeOnMainThread (() => { 
					var alert = new UIAlertView ("Error".t(), "ErrorMessage".t(), null, "Ok", null);
					//alert.Clicked += (sender, e) => UIApplication.SharedApplication.PerformSelector(new Selector("terminateWithSuccess"), null, 0f);
					alert.Show ();
				}));			
			} else {
				var publications = _parser.Publications;
				TableView.Source = new PublicationsViewSource (publications, NavigationController);
				TableView.ReloadData ();
				if (loading != null)
					loading.DismissWithClickedButtonIndex (0, true);
			}
			RefreshControl.EndRefreshing ();
		}
	}
}

