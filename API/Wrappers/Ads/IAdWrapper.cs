using UnityEngine;
using System.Collections;
using Zedarus.Traffico.Helpers.ScreenAndResolution;

namespace Zedarus.ToolKit.API
{
	#if API_IAD_P31
	public class IAdWrapper : APIWrapper<IAdWrapper>, IBannerAdsWrapperInterface
	{
		#region Settings
		#endregion
		
		#region Setup
		protected override void Setup()
		{
			#if UNITY_IPHONE
			#endif
		}
		#endregion
		
		#region Controls
		public void CreateBanner()
		{
		}
		
		public void ShowBanner()
		{
			#if UNITY_IPHONE
			AdBinding.createAdBanner(false);
			#endif
		}
		
		public void HideBanner()
		{
			#if UNITY_IPHONE
			AdBinding.destroyAdBanner();
			#endif
		}
		#endregion
		
		#region Queries
		public float GetBannerHeight()
		{
			#if UNITY_IPHONE
			if (ScreenHelper.IsIpad())
				return 33f * 2f + 8f;
			else
				return 32f * 2f + 8f;
			#else
			return 0f;
			#endif
		}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners()
		{

		}
		
		protected override void RemoveEventListeners()
		{

		}
		#endregion
		
		#region Event Handlers
		#endregion
	}
	#endif
}
