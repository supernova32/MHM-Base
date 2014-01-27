using Android.App;
using Android.Content;
using Android.Support.V4.Content;

namespace JPA.Android
{
	[BroadcastReceiver(Permission= "com.google.android.c2dm.permission.SEND")]
	[IntentFilter(new string[] { "com.google.android.c2dm.intent.RECEIVE" }, Categories = new string[] {"@PACKAGE_NAME@" })]
	class GCMBroadcastReceiver : WakefulBroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			var comp = new ComponentName (context.PackageName, typeof(GCMIntentService).Name);
			StartWakefulService (context, intent.SetComponent (comp));
			ResultCode = Result.Ok;
		}
	}
}

