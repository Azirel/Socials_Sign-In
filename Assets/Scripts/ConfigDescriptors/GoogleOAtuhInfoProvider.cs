using System;
using System.Collections.Specialized;
using System.Text;
using UnityEngine;

namespace Azirel.Config
{
	[CreateAssetMenu(fileName = "GoogleOAuthData", menuName = "OAuthData/Google", order = -1)]
	public class GoogleOAtuhInfoProvider : ScriptableObject
	{
		public string UserConsentUri = "https://accounts.google.com/o/oauth2/v2/auth";
		public string Scope = "email%20profile";
		public string ResponseType = "code";
		public string ClientID = "899582685715-8jrhr9h26gn4fpv2pmdslp1uh0b7dp7n.apps.googleusercontent.com";
		public string RedirectUri = "";
		public string GrantType = "authorization_code";

		public Uri BuildGoogleOAuthUserConsentUri() => new Uri($"{UserConsentUri}{GetQueriesAsString()}");

		public virtual string GetQueriesAsString()
			=> QueryStringsUtilities.ConvertQueriesToString(GetQueriesForConsentUri());

		public virtual NameValueCollection GetQueriesForConsentUri()
			=> new NameValueCollection()
			{
				{ "scope", Scope },
				{ "response_type", ResponseType },
				{ "client_id", ClientID },
				{ "redirect_uri", RedirectUri }
			};

		public virtual NameValueCollection GetQueriesForCodeExchange()
			=> new NameValueCollection()
			{
				{ "client_id", ClientID },
				{ "redirect_uri", RedirectUri },
				{ "grant_type", GrantType }
			};
	}
}