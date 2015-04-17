using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.Traffico.Settings;
using Zedarus.Traffico.Data.PlayerData;

namespace Zedarus.ToolKit.API
{
	public class BannerAdsController : APIController 
	{	
		#region Parameters
		#endregion
		
		#region Initialization
		public BannerAdsController(MultipleAPIUseMode useMode, params APIs[] values) : base(useMode, values) {}
		protected override void Setup() {}	
		#endregion
		
		#region Wrappers Initialization
		protected override IAPIWrapperInterface GetWrapperForAPI(APIs wrapperAPI)
		{
			switch (wrapperAPI)
			{
				case APIs.AdMob:
					return AdMobWrapper.Instance;
				#if API_IAD_P31
				case APIs.AppleiAds:
					return IAdWrapper.Instance;
				#endif
				default:
					return null;
			}
		}
		#endregion
		
		#region Controls
		public void CreateBanner()
		{
			IBannerAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				wrapper.CreateBanner();
		}
		
		public void ShowBanner()
		{
			IBannerAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				wrapper.ShowBanner();
		}
		
		public void HideBanner()
		{
			IBannerAdsWrapperInterface wrapper = Wrapper;
			if (wrapper != null)
				wrapper.HideBanner();
		}

		public void DisableAds()
		{
			HideBanner();
		}
		#endregion

		#region Queries
		public float GetBannerHeight()
		{
			IBannerAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				return wrapper.GetBannerHeight();
			else
				return 0f;
		}
		#endregion
		
		#region Getters
		protected IBannerAdsWrapperInterface Wrapper
		{
			get { return (IBannerAdsWrapperInterface)CurrentWrapperBase; }
		}
		#endregion

		#region Helpers
		private bool Enabled
		{
			get 
			{
				return GlobalSettings.Instance.AdsEnabled && PlayerDataManager.Instance.AdsEnabled;
			}
		}
		#endregion
	}
}
