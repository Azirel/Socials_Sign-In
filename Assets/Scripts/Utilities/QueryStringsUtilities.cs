using System.Collections.Specialized;
using System.Text;

public static class QueryStringsUtilities
{
	public static string ConvertQueriesToString(NameValueCollection queries)
	{
		var stringBuilder = new StringBuilder();
		stringBuilder.Append('?');
		stringBuilder.Append($"{queries.AllKeys[0]}={queries[0]}");
		queries.Remove(queries.AllKeys[0]);
		foreach (var key in queries.AllKeys)
			stringBuilder.Append($"&{key}={queries[key]}");
		return stringBuilder.ToString();
	}

	public static NameValueCollection ConvertStringQueries(string queries)
		=> System.Web.HttpUtility.ParseQueryString(queries);
}