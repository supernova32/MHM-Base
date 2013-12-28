using System.Drawing;
using MonoTouch.UIKit;
using MHMBase;
using SQLite;

namespace JPA.iOS_Normal
{
	public class PublicationsViewController : UITableViewController
	{
		SplashScreen splash;
		bool showingSplashEarlier = false;
		bool _fromMenu = false;
		readonly PublicationsParser _parser;
		readonly SQLiteConnection _db;

		public PublicationsViewController (bool skipSplash, SQLiteConnection db, bool fromMenu = false) : base (UITableViewStyle.Plain)
		{
			_parser = new PublicationsParser ();
			showingSplashEarlier = skipSplash;
			_fromMenu = fromMenu;
			_db = db;
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
				var publications = _parser.GetPublications (_db);
				TableView.Source = new PublicationsViewSource (publications, this, _db);
				TableView.ReloadData ();
			} else {
				RefreshTable ();			
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "LatestJobTitle".t();
			TableView.BackgroundColor = UIColor.FromWhiteAlpha(0.95f, 1.0f);
			RefreshControl = new UIRefreshControl ();
			RefreshControl.ValueChanged += (sender, e) => RefreshTable ();
		}

		public static bool NetworkAvailable() {
			NetworkStatus status = Reachability.InternetConnectionStatus();
			return status != NetworkStatus.NotReachable;
		}

		public void RefreshTable(UIAlertView loading = null) {
			if (NetworkAvailable ()) {
				_parser.UpdatePublications(publications => InvokeOnMainThread (() => {
					TableView.Source = new PublicationsViewSource (publications, this, _db);
					TableView.ReloadData ();
					if (loading != null)
						loading.DismissWithClickedButtonIndex (0, true);
				}), _db);			
			} else {
				var publications = _parser.GetPublications (_db);
				TableView.Source = new PublicationsViewSource (publications, this, _db);
				TableView.ReloadData ();
				if (loading != null)
					loading.DismissWithClickedButtonIndex (0, true);
			}
			RefreshControl.EndRefreshing ();
		}
	}
}

