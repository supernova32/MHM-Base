using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MHMBase;

namespace JPA.iOS_Normal
{
	public partial class PublicationViewController : UIViewController
	{
		readonly Publication _publication;
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public PublicationViewController (Publication pub)
			: base (UserInterfaceIdiomIsPhone ? "PublicationViewController_iPhone" : "PublicationViewController_iPad", null)
		{
			_publication = pub;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var db = DatabaseHelper.Instance.Connection;
			var company = db.Table<Company> ().Where (c => c.Name.Equals(_publication.Company)).First();
			Title = _publication.Title;
			PubTitle.Text = _publication.Title;
			PubTitle.TextAlignment = UITextAlignment.Center; 
			CompanyImage.Image = UIImage.FromFile (company.IconPath);
			PubDescription.Editable = false;
			PubDescription.Text = _publication.FullDescription;
			ButtonApply.SetTitle ("Apply".t(), UIControlState.Normal);
			ButtonApply.BackgroundColor = UIColor.FromRGB (135, 135, 135);
			ButtonApply.TouchUpInside += (sender, e) => {
				new UIAlertView("Touch2", "TouchUpInside handled", null, "OK", null).Show();
			};
			
			// Perform any additional setup after loading the view, typically from a nib.
		}
	}
}

