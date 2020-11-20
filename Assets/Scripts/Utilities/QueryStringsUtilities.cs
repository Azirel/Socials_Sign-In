using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

public static class QueryStringsUtilities
{
	private static readonly Regex _regex = new Regex(@"[?&](\w[\w.]*)=([^?&]+)");

	public static string ConvertQueriesToString(this NameValueCollection queries)
	{
		var stringBuilder = new StringBuilder();
		stringBuilder.Append('?');
		stringBuilder.Append($"{queries.AllKeys[0]}={queries[0]}");
		queries.Remove(queries.AllKeys[0]);
		foreach (var key in queries.AllKeys)
			stringBuilder.Append($"&{key}={queries[key]}");
		return stringBuilder.ToString();
	}

	public static NameValueCollection ConvertStringQueries(this Uri uri)
	{
		var match = _regex.Match(uri.PathAndQuery);
		var paramaters = new NameValueCollection();
		while (match.Success)
		{
			paramaters.Add(match.Groups[1].Value, match.Groups[2].Value);
			match = match.NextMatch();
		}
		return paramaters;
	}	
}