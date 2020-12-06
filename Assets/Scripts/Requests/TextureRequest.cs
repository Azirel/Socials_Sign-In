using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace Requests
{
	public class TextureRequest
	{
		protected WebRequest _request;

		public TextureRequest(string uri)
		{
			_request = HttpWebRequest.CreateHttp(uri);
			_request.Method = "GET";
		}

		public async Task<Texture> RequestImage()
		{
			var response = await _request.GetResponseAsync();
			var resultTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
			using(var resultStream = new MemoryStream())
			{
				await response.GetResponseStream().CopyToAsync(resultStream);
				resultTexture.LoadImage(resultStream.ToArray());
			}
			response.Dispose();
			return resultTexture;
		}
	}
}
