using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MHMBase;
using System.Drawing;

namespace JPA.iOS_Normal
{
	public partial class PublicationDialog : DialogViewController
	{
		public PublicationDialog (Publication pub) : base (UITableViewStyle.Plain, null, true)
		{
			var db = DatabaseHelper.Instance.Connection;
			var company = db.Table<Company> ().Where (c => c.Name.Equals(pub.Company)).First();
			//Title = pub.Title;
			var description = new MultilineElement (pub.FullDescription);
			var image = new UIImageView (new RectangleF (80, 0, 150, 150));
			image.Image = UIImage.FromFile (company.IconPath);
			var button = UIButton.FromType(UIButtonType.RoundedRect);
			button.Frame = new RectangleF(0, 0, 320, 44);
			button.BackgroundColor = UIColor.FromRGB (135, 135, 135);
			button.SetTitleColor (UIColor.White, UIControlState.Normal); 
			button.SetTitle ("Apply".t(), UIControlState.Normal);
			button.TouchUpInside += (sender, e) => NavigationController.PushViewController (new WebViewController (pub), true);
			Root = new RootElement (pub.Title) {
				new Section () {
					new UIViewElement ("", image, false),
					new StyledStringElement (pub.Title),
					description,
					new UIViewElement ("", button, false)
				}
			};
		}
	}
}
