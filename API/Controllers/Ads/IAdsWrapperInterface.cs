using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public interface IAdsWrapperInterface
	{
		#region Events
		event Action InterstitialClosed;
		event Action GrantReward;
		event Action BannerDisplayed;
		event Action BannerRemoved;
		#endregion

		#region Controls - Caching
		void CacheInterstitial(string tag);
		void CacheRewardedVideo(string tag);
		#endregion

		#region Controls
		void ShowBanner(string tag);
		void HideBanner();
		void ShowIntersitital(string tag);
		void ShowRewardedVideo(string tag);
		void ShowTestUI();
		#endregion

		#region Queries
		float GetBannerHeight();
		bool IsBannerVisible();
		#endregion
	}
}
