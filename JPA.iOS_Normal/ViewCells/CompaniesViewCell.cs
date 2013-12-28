using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace JPA.iOS_Normal
{
	public class CompaniesViewCell : UICollectionViewCell
	{
		UIImageView imageView;
		public UITextField DisplayLabel { get; set; }
		public static readonly NSString Key = new NSString ("CompaniesViewCell");

		[Export ("initWithFrame:")]
		public CompaniesViewCell (RectangleF frame) : base (frame)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			BackgroundColor = UIColor.FromWhiteAlpha(0, 1.0f);

			BackgroundView = new UIView{BackgroundColor = UIColor.Orange};

			SelectedBackgroundView = new UIView{BackgroundColor = UIColor.Green};

			ContentView.Layer.BorderColor = UIColor.LightGray.CGColor;
			ContentView.Layer.BorderWidth = 1.0f;
			ContentView.BackgroundColor = UIColor.White;
			ContentView.Transform = CGAffineTransform.MakeScale (0.9f, 0.9f);

			imageView = new UIImageView (UIImage.FromBundle ("Images/placeholder.png"));
			imageView.Center = ContentView.Center;
			DisplayLabel = new UITextField () 
			{
				BackgroundColor = UIColor.White,
				//Font = UIFont.FromName ("SourceSansPro-Bold", 15f),
				Frame = new RectangleF (0, 0, 129, 18),
				TextAlignment = UITextAlignment.Center,
				//TextColor = UIColor.LightGray
			};
			//imageView.Transform = CGAffineTransform.MakeScale (0.7f, 0.7f);

			ContentView.AddSubview (imageView);
			ContentView.Add (DisplayLabel);
		}

		public UIImage Image {
			set {
				imageView.Image = value;
			}
		}
	}
}

