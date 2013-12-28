// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace JPA.iOS_Normal
{
	[Register ("SplashScreen")]
	partial class SplashScreen
	{
		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView aiSplash { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView splashImage { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (splashImage != null) {
				splashImage.Dispose ();
				splashImage = null;
			}

			if (aiSplash != null) {
				aiSplash.Dispose ();
				aiSplash = null;
			}
		}
	}
}
