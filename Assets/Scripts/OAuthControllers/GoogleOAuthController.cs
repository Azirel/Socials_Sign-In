using Azirel.Config;
using Azirel.View;
using Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.Networking;

namespace Azirel.Controllers
{
	public class GoogleOAuthController : MonoBehaviour
	{
		public GoogleOAtuhInfoProvider GoogleOAuthConfig;
		public SignInViewControllerBase ViewImplementation;

		protected ISignInViewBase _view;

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
			=> _ = HandleDeeplinkRedirectAsync(link);

		protected virtual async Task HandleDeeplinkRedirectAsync(string link)
		{
			var code = RetrieveCodeFromRedirectDeeplink(link);
			var queriesForCodeExchange = BuildGoogleOAuthCodeExchangeQueries(code);
			var idToken = await ExchangeCodeForIdToken(queriesForCodeExchange);
			var idTokenInfo = await RequestIdTokenInfo(idToken);
			ViewTokenInfo(idTokenInfo);
		}

		protected virtual string RetrieveCodeFromRedirectDeeplink(string deeplink)
		{
			var queriesCollection = QueryStringsUtilities.ConvertStringQueries(new Uri(deeplink));
			return queriesCollection["code"];
		}

		protected virtual NameValueCollection BuildGoogleOAuthCodeExchangeQueries(string code)
			=> new NameValueCollection()
				{
					{ "client_id", GoogleOAuthConfig.ClientID },
					{ "redirect_uri", GoogleOAuthConfig.RedirectUri },
					{ "grant_type", "authorization_code" },
					{ "code",  code }
				};

		protected virtual async Task<string> ExchangeCodeForIdToken(NameValueCollection queries)
		{
			var _codeExchangeRequest = new GoogleOAuthCodeExchangeRequest(queries);
			await _codeExchangeRequest.RequestExchange();
			return _codeExchangeRequest.IdToken;
		}

		protected virtual async Task<string> RequestIdTokenInfo(string idToken)
		{
			var _tokenInfoRequest = new GoogleIdTokenInfoRequest(idToken);
			return await _tokenInfoRequest.RequestIdTokenInfo();			
		}

		protected virtual void ViewTokenInfo(string tokenInfoJson)
		{
			var nameValuePairs = new List<Tuple<string, string>>();
			foreach (var item in JObject.Parse(tokenInfoJson))
				nameValuePairs.Add(new Tuple<string, string>(item.Key, item.Value.ToString()));
			var imagesUris = RetrieveImagesIfExist(nameValuePairs);
			_view.MainInfo = nameValuePairs;
			var request = UnityWebRequestTexture.GetTexture(imagesUris.First(), true);
			request.SendWebRequest().completed += (operation)
				=> _view.MainImage = ((DownloadHandlerTexture)request.downloadHandler).texture;
		}

		protected IEnumerable<string> RetrieveImagesIfExist(IEnumerable<Tuple<string, string>> fields)
			=> fields.Where((field) => field.Item2.EndsWith(".jpg") || field.Item2.EndsWith(".png")).Select((field) => field.Item2);
	} 
}