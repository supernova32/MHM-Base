using Android.Content;

namespace JPA.Android
{
	public sealed class SuggestionProvider : SearchRecentSuggestionsProvider
	{
		public static string Authority = "jpa.android.SuggestionProvider";
		public static DatabaseMode Mode = DatabaseMode.Queries;

		public SuggestionProvider () {
			SetupSuggestions (Authority, Mode);		
		}
	}
}

