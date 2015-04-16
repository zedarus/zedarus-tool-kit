using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public interface IBannerAdsWrapperInterface
	{
		#region Events
		#endregion
		
		#region Controls
		void CreateBanner();
		void ShowBanner();
		void HideBanner();
		#endregion
		
		#region Queries
		float GetBannerHeight();
		#endregion
	}
}
