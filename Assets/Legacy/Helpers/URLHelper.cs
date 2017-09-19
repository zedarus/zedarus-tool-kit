using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Helpers
{
	public class URLHelper
	{
		static public void Open(string url, bool inBrowser)
		{
			if (inBrowser)
				Application.OpenURL(url);
			else
			{
				#if PRIME31
				#if UNITY_IPHONE
			//EtceteraBinding.showWebPage(url, true);
				#endif
				#endif
			}
		
			//APIManager.Instance.Analytics.LogExternalLink(url, inBrowser);
		}
	}
}

