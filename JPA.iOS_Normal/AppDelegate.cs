using MonoTouch.Foundation;
using MonoTouch.SlideoutNavigation;
using MonoTouch.UIKit;
using System;

namespace JPA.iOS_Normal
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		public SlideoutNavigationController Menu { get; private set; }
		UIWindow window;
		//private UINavigationController _navigationCotroller;
		//
		// This method is invoked when the application has loaded and is ready to run. In this
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			Menu = new SlideoutNavigationController ();
			Menu.TopView = new PublicationsViewController (false);
			Menu.LeftMenuButtonText = "Menu".t();
			Menu.MenuViewLeft = new LeftMenu ();
			Menu.DefineTextColor ();
			Menu.BarTintColor = UIColor.FromRGB (255, 153, 0);
			Menu.TintColor = UIColor.White;
			Menu.Translucent = false; 
			window.RootViewController = Menu;
			
			// make the window visible
			window.MakeKeyAndVisible ();
			UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (UIRemoteNotificationType.Alert 
				| UIRemoteNotificationType.Badge 
				| UIRemoteNotificationType.Sound);

			if (launchOptions != null) {
				var dictionary = launchOptions.ObjectForKey (UIApplication.LaunchOptionsRemoteNotificationKey);
				if (dictionary != null) {
					Console.WriteLine ("Received Dictionary: " + dictionary);
					// TODO Handle the information from the notification and show
					// the appropriate view to the user				
				}
			}
			
			return true;
		}

		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)		
		{
			TokenHandler.SetToken (deviceToken);
		}

		public override void DidReceiveRemoteNotification (UIApplication application, NSDictionary userInfo, System.Action<UIBackgroundFetchResult> completionHandler)
		{
			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/guides/ios/application_fundamentals/delegates,_protocols,_and_events
			Console.WriteLine ("Received info: " + userInfo);
		}

	}
}

