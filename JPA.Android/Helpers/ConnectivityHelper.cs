using Android.App;
using Android.Net;

namespace JPA.Android
{
	class ConnectivityHelper
	{
		static ConnectivityHelper currentInstance;
		readonly ConnectivityManager connectivityManager;

		public static ConnectivityHelper Instance (Activity context) {
			if (currentInstance == null)
				currentInstance = new ConnectivityHelper (context);
			return currentInstance;		
		}

		ConnectivityHelper (Activity context) {
			connectivityManager	= (ConnectivityManager)context.GetSystemService ("connectivity");		
		}

		public bool NetworkAvailable ()
		{
			var activeConnection = connectivityManager.ActiveNetworkInfo;
			if ((activeConnection != null) && activeConnection.IsConnected) {
				return true;
			}
			return false;		
		}
	}
}

