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
	[Register ("PublicationViewController")]
	partial class PublicationViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton ButtonApply { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView CompanyImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView PubDescription { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PubTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView ScrollView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CompanyImage != null) {
				CompanyImage.Dispose ();
				CompanyImage = null;
			}

			if (PubDescription != null) {
				PubDescription.Dispose ();
				PubDescription = null;
			}

			if (PubTitle != null) {
				PubTitle.Dispose ();
				PubTitle = null;
			}

			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}

			if (ButtonApply != null) {
				ButtonApply.Dispose ();
				ButtonApply = null;
			}
		}
	}
}
