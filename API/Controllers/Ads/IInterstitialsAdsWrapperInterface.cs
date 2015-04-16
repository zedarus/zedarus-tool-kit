using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public interface IInterstitialsAdsWrapperInterface
	{
		#region Events
		event Action InterstitialClosed;
		#endregion
		
		#region Controls
		void CacheBootupAd();
		void CacheBetweenLevelAd();
		void CacheMoreGamesAd();
		void ShowBootupAd();
		void ShowBetweenLevelAd();
		void ShowMoreGamesAd();
		#endregion
		
		#region Queries
		#endregion
	}
}
