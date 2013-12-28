using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace JPA.iOS_Normal
{
	public partial class CompaniesXibView : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("CompaniesXibView");
		public static readonly UINib Nib;

		static CompaniesXibView ()
		{
			Nib = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? UINib.FromName ("CompaniesXibView_iPhone", NSBundle.MainBundle) : UINib.FromName ("CompaniesXibView_iPad", NSBundle.MainBundle);
		}

		public CompaniesXibView (IntPtr handle) : base (handle)
		{
			imageView = new UIImageView (UIImage.FromBundle ("Images/placeholder.png"));
		}

		public static CompaniesXibView Create ()
		{
			return (CompaniesXibView)Nib.Instantiate (null, null) [0];
		}

		public UIImage Image {
			set {
				imageView.Image = value;
			}
		}

		public string CompanyName {
			set {
				companyName.Text = value;
			}
		}
	}
}

