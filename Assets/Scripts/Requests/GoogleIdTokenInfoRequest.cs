using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Requests
{
	public class GoogleIdTokenInfoRequest
	{
		public Uri RequestUri => tokenInfoUri;
		public event Action OnComplete;

		protected Uri tokenInfoUri;
		protected string rawResponse;
		protected string Method { get; set; } = "GET";
		
		protected WebRequest _request;

		protected virtual string ConvertResponseStream(Stream sourceStream)
		{
			using (var reader = new StreamReader(sourceStream, Encoding.UTF8))
				return reader.ReadToEnd();
		}

		public GoogleIdTokenInfoRequest(string idToken)
		{
			var queriesString = QueryStringsUtilities.ConvertQueriesToString(new System.Collections.Specialized.NameValueCollection() { { "id_token", idToken} });
			tokenInfoUri = new Uri($"https://oauth2.googleapis.com/tokeninfo{queriesString}");
			_request = HttpWebRequest.CreateHttp(tokenInfoUri);
			_request.Method = Method;
			_request.ContentLength = 0;
		}

		public async Task<string> RequestIdTokenInfo()
		{
			WebResponse response = null;
			try { response = await _request.GetResponseAsync(); }
			catch (Exception e) { Debug.Log(e); }
			rawResponse = ConvertResponseStream(response.GetResponseStream());
			response.GetResponseStream().Dispose();			
			OnComplete?.Invoke();
			return rawResponse;
		}
	}
}