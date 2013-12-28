using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using SQLite;

namespace JPA.iOS_Normal
{
	public partial class LeftMenu : DialogViewController
	{
		public LeftMenu (SQLiteConnection db) : base (UITableViewStyle.Grouped, null)
		{
			var CompaniesLayout = new CompaniesLayout ();
			Root = new RootElement ("Menu") {
				new Section ("Navigation".t()) {
					new BadgeElement (UIImage.FromBundle ("Images/placeholder.png"), "Home".t(), () => {
						NavigationController.PushViewController(new PublicationsViewController (true, db, true), true); 
					}),
					new BadgeElement (UIImage.FromBundle ("Images/placeholder.png"), "Companies".t(), () => {
						NavigationController.PushViewController(new CompaniesViewController(CompaniesLayout, db), true);
					})
				},
				new Section ("Second Section") {
					new BadgeElement (UIImage.FromBundle ("Images/placeholder.png"), "Settings".t(), () => {
						NavigationController.PushViewController(new Settings(), true);
					}),
					new BadgeElement (UIImage.FromBundle ("Images/placeholder.png"), "Search".t(), () => NavigationController.PushViewController (new SearchViewController (db), true))
				},
			};
		}
	}
}
