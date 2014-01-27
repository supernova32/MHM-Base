using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using SQLite;
using MHMBase;

namespace JPA.iOS_Normal
{
	public class PublicationsViewSource : UITableViewSource
	{
		readonly IList<Publication> _publications; 
		const string PublicationCell = "PublicationCell";
		readonly UINavigationController _controller;
		readonly SQLiteConnection _db;

		public PublicationsViewSource (IList<Publication> publications, UINavigationController controller) {
			_publications = publications;
			_controller = controller;
			_db = DatabaseHelper.Instance.Connection;
		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return _publications.Count;;
		}

//		public override string TitleForHeader (UITableView tableView, int section)
//		{
//			return "Header".t();;
//		}

//		public override string TitleForFooter (UITableView tableView, int section)
//		{
//			return "Footer";
//		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (PublicationsViewCell.Key) as PublicationsViewCell ?? new PublicationsViewCell ();
			var publication = _publications[indexPath.Row];

			cell.TextLabel.Text = publication.Title;
			var company = _db.Table<Company> ().Where (c => c.Name.Equals (publication.Company)).First ();
			cell.ImageView.Image = UIImage.FromFile (company.IconPath);//FromUrl (publication.Icon);
			cell.DetailTextLabel.Text = publication.ShortDescription;

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			_controller.PushViewController (new PublicationDialog (_publications[indexPath.Row]), true);
		}

//		static UIImage FromUrl (string uri)
//		{
//			using (var url = new NSUrl (uri))
//			using (var data = NSData.FromUrl (url))
//				return UIImage.LoadFromData (data);
//		}
	}
}

