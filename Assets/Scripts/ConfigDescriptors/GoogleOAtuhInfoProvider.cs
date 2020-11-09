using System;
using System.Collections.Specialized;
using System.Text;
using UnityEngine;

namespace Azirel.Config
{
	[CreateAssetMenu(fileName = "GoogleOAuthData", menuName = "OAuthData/Google", order = -1)]
	public class GoogleOAtuhInfoProvider : ScriptableObject
	{
		public string Uri = "https://accounts.google.com/o/oauth2/v2/auth";
		public string Scope = "email%20profile";
		public string ResponseType = "code";
		public string ClientID = "899582685715-8jrhr9h26gn4fpv2pmdslp1uh0b7dp7n.apps.googleusercontent.com";
		public string RedirectUri = "com.azirel.socials.signup://sign-up";

		public Uri BuildGoogleOAuthUserConsentUri() => new Uri($"{Uri}{GetQueries()}");

		protected virtual string GetQueries()
			=> QueryStringsUtilities.ConvertQueriesToString(BuildQueries());

		protected NameValueCollection BuildQueries()
			=> new NameValueCollection()
			{
				{ "scope", Scope },
				{ "response_type", ResponseType },
				{ "client_id", ClientID },
				{ "redirect_uri", RedirectUri }
			};
	} 
}