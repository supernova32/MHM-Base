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
	[Register ("CompaniesXibView")]
	partial class CompaniesXibView
	{
		[Outlet]
		MonoTouch.UIKit.UILabel companyName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (companyName != null) {
				companyName.Dispose ();
				companyName = null;
			}

			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}
		}
	}
}
