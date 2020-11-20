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
			var idToken = await ExchangeCodeForIdToken(code);
			var idTokenInfo = await RequestIdTokenInfo(idToken);
			await ViewTokenInfo(idTokenInfo);
		}

		protected virtual string RetrieveCodeFromRedirectDeeplink(string deeplink)
		{
			var queriesCollection = QueryStringsUtilities.ConvertStringQueries(new Uri(deeplink));
			return queriesCollection["code"];
		}		

		protected virtual async Task<string> ExchangeCodeForIdToken(string code)
		{
			var _codeExchangeRequest = new GoogleOAuthCodeForIdTokenExchangeRequest(code);
			return await _codeExchangeRequest.RequestIdToken();
		}

		protected virtual async Task<string> RequestIdTokenInfo(string idToken)
		{
			var _tokenInfoRequest = new GoogleIdTokenInfoRequest(idToken);
			return await _tokenInfoRequest.RequestIdTokenInfo();			
		}

		protected virtual async Task ViewTokenInfo(string tokenInfoJson)
		{
			var nameValuePairs = new List<Tuple<string, string>>();
			foreach (var item in JObject.Parse(tokenInfoJson))
				nameValuePairs.Add(new Tuple<string, string>(item.Key, item.Value.ToString()));
			var imagesUris = RetrieveImagesIfExist(nameValuePairs);
			_view.MainInfo = nameValuePairs;
			var textureRequest = new TextureRequest(imagesUris.First());			
			_view.MainImage = await textureRequest.RequestImage();
		}

		protected IEnumerable<string> RetrieveImagesIfExist(IEnumerable<Tuple<string, string>> fields)
			=> fields.Where((field) => field.Item2.EndsWith(".jpg") || field.Item2.EndsWith(".png")).Select((field) => field.Item2);
	} 
}