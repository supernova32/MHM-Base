using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using PerpetualEngine.Storage;

namespace JPA.iOS_Normal
{
	public partial class Settings : DialogViewController
	{
		readonly BooleanElement _notifications;
		readonly BooleanElement _settings;
		readonly Section _network;
		public Settings () : base (UITableViewStyle.Grouped, null)
		{
			var storage = SimpleStorage.EditGroup("preferences");
			Root = new RootElement ("Settings".t ());
			_network = new Section ("Network".t ());
			var notification = storage.Get ("notifications");
			_notifications = new BooleanElement ("NotificationsAllow".t (), Convert.ToBoolean (notification));
			_notifications.ValueChanged += (sender, e) => {
				if (_notifications.Value) {
					storage.Put ("notifications", "True");
				} else {
					storage.Put ("notifications", "False");
				}
			};
			_network.Add (_notifications);
			Root.Add (_network);
		}
	}
}
