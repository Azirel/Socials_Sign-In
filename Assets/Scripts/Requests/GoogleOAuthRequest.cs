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

namespace Requests
{
	public class GoogleOAuthCodeForIdTokenExchangeRequest
	{
		protected WebRequest _request;

		protected virtual string ConvertResponseStreamToText(Stream sourceStream)
		{
			using (var reader = new StreamReader(sourceStream, Encoding.UTF8))
				return reader.ReadToEnd();
		}

		public GoogleOAuthCodeForIdTokenExchangeRequest(NameValueCollection queries)
		{
			var queriesString = queries.ConvertQueriesToString();
			var tokenExchangeUri = new Uri($"https://oauth2.googleapis.com/token{queriesString}");
			_request = HttpWebRequest.CreateHttp(tokenExchangeUri);
			_request.Method = "POST";
			_request.ContentLength = 0;
		}

		public GoogleOAuthCodeForIdTokenExchangeRequest(NameValueCollection queries, string code)
		{
			queries.Add("code", code);
			var queriesString = queries.ConvertQueriesToString();
			var tokenExchangeUri = new Uri($"https://oauth2.googleapis.com/token{queriesString}");
			_request = HttpWebRequest.CreateHttp(tokenExchangeUri);
			_request.Method = "POST";
			_request.ContentLength = 0;
		}

		public async Task<string> RequestIdToken()
		{
			var response = await _request.GetResponseAsync();
			var responseStream = response.GetResponseStream();
			var rawResponse = ConvertResponseStreamToText(responseStream);
			responseStream.Dispose();
			return RetrieveIdTokenFromJson(rawResponse);
		}

		protected virtual string RetrieveIdTokenFromJson(string json)
			=> JObject.Parse(json)["id_token"].Value<string>();
	} 
}