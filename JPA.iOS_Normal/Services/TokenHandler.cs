using System;
using MonoTouch.Foundation;
using PerpetualEngine.Storage;
using MonoTouch.ObjCRuntime;

namespace JPA.iOS_Normal
{
	public static class TokenHandler
	{
		const string PropertyRegID = "registration_id";


		public static void SetToken (NSData deviceToken)
		{
			var storage = SimpleStorage.EditGroup("preferences");
			var lastDeviceToken = storage.Get (PropertyRegID);
			var strFormat = new NSString("%@");
			var dt = new NSString(Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr(new Class("NSString").Handle, new Selector("stringWithFormat:").Handle, strFormat.Handle, deviceToken.Handle));
			var newDeviceToken = dt.ToString().Replace("<", "").Replace(">", "").Replace(" ", "");

			if (!newDeviceToken.Equals (lastDeviceToken)) {
				storage.Put (PropertyRegID, newDeviceToken);
				Console.WriteLine ("New Device Token saved: " + newDeviceToken);			
			} else {
				Console.WriteLine ("Using old token: " + lastDeviceToken);			
			}
		}
	}
}

