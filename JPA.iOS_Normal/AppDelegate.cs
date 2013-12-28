using MonoTouch.Foundation;
using MonoTouch.SlideoutNavigation;
using MonoTouch.UIKit;
using SQLite;
using MHMBase;

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
		readonly SQLiteConnection db = DatabaseHelper.Instance.Connection;
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
			Menu.TopView = new PublicationsViewController (false, db);
			Menu.LeftMenuButtonText = "Menu";
			Menu.MenuViewLeft = new LeftMenu (db);//new DummyControllerLeft (db);
//			_navigationCotroller = new UINavigationController();
//			_navigationCotroller.PushViewController (new PublicationsViewController(), false);
			window.RootViewController = Menu;//_navigationCotroller;
			
			// make the window visible
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

