using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace JPA.iOS_Normal
{
	public partial class PublicationsViewCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("PublicationsViewControllerCell");

		public PublicationsViewCell () : base (UITableViewCellStyle.Subtitle, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			Accessory = UITableViewCellAccessory.DisclosureIndicator;
			DetailTextLabel.Text = "Details:";
			BackgroundColor = UIColor.FromWhiteAlpha(0.95f, 1.0f);
		}
	}
}

