using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace JPA.iOS_Normal
{
	public class CompaniesLayout : UICollectionViewFlowLayout
	{
		int NumberOfColumns { get; set; }
		NSDictionary LayoutInfo { get; set; }

		public CompaniesLayout ()
		{
			ItemSize = new SizeF (80.0f, 115.0f);
			SectionInset = new UIEdgeInsets (22.0f, 22.0f, 13.0f, 22.0f);
			MinimumLineSpacing = 12.0f;
			NumberOfColumns = 2;
		}

		public override bool ShouldInvalidateLayoutForBoundsChange (RectangleF newBounds)
		{
			return true;
		}

	}
}

