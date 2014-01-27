using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Gms.Gcm;
using System.Threading;

namespace JPA.Android
{
	[Service]
	class GCMIntentService : IntentService
	{
		public const int NotificationID = 1;
		NotificationManager NotificationManager;
		NotificationCompat.Builder Builder;

		public GCMIntentService () : base ("GCMIntentService") {}
		public static String TAG = "MHM GCM Service";

		protected override void OnHandleIntent (Intent intent)
		{
			var extras = intent.Extras;
			var gcm = GoogleCloudMessaging.GetInstance(this);
			// The getMessageType() intent parameter must be the intent you received
			// in your BroadcastReceiver.
			var messageType = gcm.GetMessageType(intent);

			if (!extras.IsEmpty) {  // has effect of unparcelling Bundle
				/*				
             * Filter messages based on message type. Since it is likely that GCM will be
             * extended in the future with new message types, just ignore any message types you're
             * not interested in, or that you don't recognize.
             */
				if (GoogleCloudMessaging.MessageTypeSendError.Equals(messageType)) {
					SendNotification("Send error: " + extras);
				} else if (GoogleCloudMessaging.MessageTypeDeleted.Equals(messageType)) {
					SendNotification("Deleted messages on server: " + extras);
					// If it's a regular GCM message, do some work.
				} else if (GoogleCloudMessaging.MessageTypeMessage.Equals(messageType)) {
					// This loop represents the service doing some work.
					for (int i = 0; i < 5; i++) {
						Console.Write(TAG, "Working... " + (i + 1)
							+ "/5 @ " + SystemClock.ElapsedRealtime());
						try {
							Thread.Sleep(5000);
						} catch (Java.Lang.InterruptedException) {
						}
					}
					Console.Write(TAG, "Completed work @ " + SystemClock.ElapsedRealtime());
					// Post notification of received message.
					SendNotification("Received: " + extras);
					Console.Write(TAG, "Received: " + extras);
				}
			}
			// Release the wake lock provided by the WakefulBroadcastReceiver.
			GCMBroadcastReceiver.CompleteWakefulIntent(intent);
		}

		void SendNotification(String msg) {
			NotificationManager = (NotificationManager) GetSystemService(Context.NotificationService);

			var contentIntent = PendingIntent.GetActivity(this, 0,
				new Intent(this, typeof(MainActivity)), 0);

			Builder = new NotificationCompat.Builder (this)
				.SetSmallIcon (Resource.Drawable.action_mhm)
				.SetContentTitle (GetString(Resource.String.app_name))
				.SetStyle (new NotificationCompat.BigTextStyle ().BigText (msg))
				.SetContentText (msg)
				.SetLights (8, 600, 500);

			Builder.SetContentIntent(contentIntent);
			NotificationManager.Notify(NotificationID, Builder.Build());
		}
	}
}

