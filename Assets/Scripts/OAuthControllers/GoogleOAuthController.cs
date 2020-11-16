using Azirel.Config;
using Azirel.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace Azirel.Controllers
{
	public class GoogleOAuthController : MonoBehaviour
	{
		public GoogleOAtuhInfoProvider GoogleOAuthConfig;
		public SignInViewControllerBase ViewImplementation;

		protected ISignInViewBase _view;
		protected GoogleOAuthCodeExchangeRequest _codeExchangeRequest;

		protected virtual void Start()
		{
			Application.deepLinkActivated += HandleDeeplinkRedirect;
			_view = ViewImplementation;
			_view.OnSignInButtonPressed += RedirectFromAppToBrowserSignInPage;
		}

		public virtual void RedirectFromAppToBrowserSignInPage()
			=> Application.OpenURL(BuildOAuthURI().AbsoluteUri);

		protected virtual Uri BuildOAuthURI()
			=> GoogleOAuthConfig.BuildGoogleOAuthUserConsentUri();

		protected virtual void HandleDeeplinkRedirect(string link)
		{
			var queriesRawString = link.Replace(GoogleOAuthConfig.RedirectUri, string.Empty);
			var queriesCollection = QueryStringsUtilities.ConvertStringQueries(queriesRawString);
			var queriesForCodeExchange = new NameValueCollection()
			{
				{ "client_id", GoogleOAuthConfig.ClientID },
				{ "redirect_uri", GoogleOAuthConfig.RedirectUri },
				{ "grant_type", "authorization_code" },
				{ "code",  queriesCollection["code"] }
			};
			_codeExchangeRequest = new GoogleOAuthCodeExchangeRequest(queriesForCodeExchange);
			_codeExchangeRequest.RequestExchange();
			_codeExchangeRequest.OnComplete += HandleCodeExchange;
		}

		protected virtual void HandleCodeExchange() => print(_codeExchangeRequest.IdToken);

		[ContextMenu("Get URI")]
		public void Test()
			=> print(BuildOAuthURI().AbsoluteUri);
	} 
}