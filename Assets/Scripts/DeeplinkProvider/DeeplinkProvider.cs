using UnityEngine;

public static class DeeplinkProvider
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	public static void OnApplicationStartHandler()
		=> Application.deepLinkActivated += DeeplinkingHandler;

	private static void DeeplinkingHandler(string link)
		=> Debug.Log($"App activated via deeplink {{{link}}}");	
}