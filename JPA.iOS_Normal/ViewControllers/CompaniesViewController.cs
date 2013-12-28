using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MHMBase;
using System.Collections.Generic;
using SQLite;

namespace JPA.iOS_Normal
{
	public partial class CompaniesViewController : UICollectionViewController
	{
		readonly IList<Company> _companies;

		public CompaniesViewController (UICollectionViewLayout layout, SQLiteConnection db) : base (layout)
		{
			var parser = new CompaniesParser ();
			_companies = parser.GetCompanies (db);

		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Title = "CompaniesTitle".t ();
			CollectionView.BackgroundColor = UIColor.FromWhiteAlpha (0.95f, 1.0f);
			
			// Register any custom UICollectionViewCell classes
			//CollectionView.RegisterClassForCell (typeof(CompaniesViewCell), CompaniesViewCell.Key);
			
			// Note: If you use one of the Collection View Cell templates to create a new cell type,
			// you can register it using the RegisterNibForCell() method like this:
			//
			CollectionView.RegisterNibForCell (CompaniesXibView.Nib, CompaniesXibView.Key);
		}

		public override int NumberOfSections (UICollectionView collectionView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override void ItemHighlighted (UICollectionView collectionView, NSIndexPath indexPath)
		{
			// TODO: Should filter publications by company => retreive from server.
//			var cell = collectionView.CellForItem (indexPath);
//			cell.ContentView.BackgroundColor = UIColor.Yellow;
			var company = _companies [indexPath.Row];
			new UIAlertView("Full Info", company.FullName, null, "Ok", null).Show();

		}

		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			return _companies.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell (CompaniesXibView.Key, indexPath) as CompaniesXibView;
			var company = _companies [indexPath.Row];
			cell.Image = UIImage.FromFile (company.IconPath);
			cell.CompanyName = company.FullName;
			
			// TODO: populate the cell with the appropriate data based on the indexPath
			
			return cell;
		}
	}	
}

