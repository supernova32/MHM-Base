using System;
using MonoTouch.Foundation;

namespace JPA.iOS_Normal
{
	public static class Extension
	{

		public static string t(this string translate)
		{
			return NSBundle.MainBundle.LocalizedString(translate, "", "");
		}
	}
}

