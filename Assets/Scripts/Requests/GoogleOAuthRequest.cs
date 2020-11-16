using UnityEngine.Networking;
using System.Net;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class GoogleOAuthCodeExchangeRequest
{
	public Uri RequestUri => tokenExchangeUri;
	public string IdToken { get; protected set; }
	public event Action OnComplete;

	protected Uri tokenExchangeUri;
	protected string rawResponse;
	protected string Method { get; set; } = "POST";
	protected virtual string ExtrudeIdTokenFromJson(string json)
		=> JObject.Parse(json)["id_token"].Value<string>();
	protected WebRequest _request;

	protected virtual string ConvertResponseStream(Stream sourceStream)
	{
		using (var reader = new StreamReader(sourceStream, Encoding.UTF8))
			return reader.ReadToEnd();
	}

	public GoogleOAuthCodeExchangeRequest(NameValueCollection queries)
	{
		var queriesString = QueryStringsUtilities.ConvertQueriesToString(queries);
		tokenExchangeUri = new Uri($"https://oauth2.googleapis.com/token{queriesString}");
		_request = HttpWebRequest.CreateHttp(tokenExchangeUri);
		_request.Method = Method;
		_request.ContentLength = 0;
	}

	public async Task RequestExchange()
	{
		WebResponse response = null;
		try { response = await _request.GetResponseAsync(); }
		catch (Exception e) { Debug.Log(e); }
		rawResponse = ConvertResponseStream(response.GetResponseStream());
		response.GetResponseStream().Dispose();
		IdToken = ExtrudeIdTokenFromJson(rawResponse);
		OnComplete?.Invoke();
	}
}