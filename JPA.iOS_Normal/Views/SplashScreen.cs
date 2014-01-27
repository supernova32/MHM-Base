using System;
using MonoTouch.UIKit;
using System.Threading;
using PerpetualEngine.Storage;
using SQLite;
using MHMBase;
using System.Threading.Tasks;
using MonoTouch.ObjCRuntime;

namespace JPA.iOS_Normal
{
	public partial class SplashScreen : UIViewController
	{
		public event Action OnReady;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public SplashScreen () : base ("SplashScreen", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			aiSplash.StartAnimating();
			splashImage.Image = UIImage.FromBundle ("Images/splash.png");
			ThreadPool.QueueUserWorkItem (o => SetUpDatabase());			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		object SetUpDatabase () {
			var storage = SimpleStorage.EditGroup("preferences");
			var value = storage.Get("companies_exist");
			if (value == null) {
				if (PublicationsViewController.NetworkAvailable ()) {
					UpdateCompanies (storage);								
				} else {
					InvokeOnMainThread (() => { 
						var alert = new UIAlertView ("Network".t(), "NetworkMessage".t(), null, "Ok", null);
						alert.Clicked += (sender, e) => UIApplication.SharedApplication.PerformSelector(new Selector("terminateWithSuccess"), null, 0f);
						alert.Show ();
					});				
				}

			} else {
				if (PublicationsViewController.NetworkAvailable ()) {
					UpdateCompanies (storage);								
				} else {
					InvokeOnMainThread (() => {
						aiSplash.StopAnimating ();
						OnReady.Invoke ();
					});
				}			
			}
			return null;		
		}

		public void UpdateCompanies (SimpleStorage storage) {
			var cp = new CompaniesParser ();
			cp.PopulateCompaniesDB (success => InvokeOnMainThread (() => {
				if (success)
					storage.Put ("companies_exist", "True");
				aiSplash.StopAnimating ();
				OnReady.Invoke ();
			}), status => InvokeOnMainThread (() => {
				var alert = new UIAlertView ("Network".t(), "NetworkMessage".t(), null, "Ok", null);
				alert.Clicked += (sender, e) => UIApplication.SharedApplication.PerformSelector(new Selector("terminateWithSuccess"), null, 0f);
				alert.Show ();
			}));
		}
	}
}

