using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace JPA.iOS_Normal
{
	public class LeftMenu : DialogViewController
	{
		public LeftMenu () : base (UITableViewStyle.Plain, null)
		{
			var CompaniesLayout = new CompaniesLayout ();
			Root = new RootElement ("Menu") {
				new Section () { //"Navigation".t()
					new BadgeElement (UIImage.FromBundle ("Images/ic_home.png"), "Home".t(), () => NavigationController.PushViewController (new PublicationsViewController (true, true), true)),
					new BadgeElement (UIImage.FromBundle ("Images/ic_companies.png"), "Companies".t(), () => NavigationController.PushViewController (new CompaniesViewController (CompaniesLayout), true)),
					new BadgeElement (UIImage.FromBundle ("Images/ic_search.png"), "Search".t(), () => NavigationController.PushViewController (new SearchViewController (), true))
				}//,
//				new Section ("Second Section") {
//					new BadgeElement (UIImage.FromBundle ("Images/placeholder.png"), "Settings".t(), () => NavigationController.PushViewController (new Settings (), true)),
//
//				},
			};
//			this.NavigationController.NavigationBar.TintColor = UIColor.FromRGB (135, 135, 135);
//			var textAttributes = new UITextAttributes ();
//			textAttributes.TextColor = UIColor.White;
//			this.NavigationController.NavigationBar.SetTitleTextAttributes (textAttributes);
		}
	}
}
