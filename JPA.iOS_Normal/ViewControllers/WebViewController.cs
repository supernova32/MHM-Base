using System;
using MonoTouch.UIKit;
using MHMBase;
using MonoTouch.Foundation;

namespace JPA.iOS_Normal
{
	public class WebViewController : UIViewController {

		readonly Publication _pub;
		UIWebView webView; 

		public override void ViewDidLoad() {
			base.ViewDidLoad ();
			webView = new UIWebView (View.Bounds);
			Title = _pub.Company;
			View.AddSubview (webView);
			webView.LoadRequest (new NSUrlRequest(new NSUrl(_pub.Link)));
			webView.ScalesPageToFit = false;
		}

		public WebViewController (Publication pub) {
			_pub = pub;		
		}

	}
}

