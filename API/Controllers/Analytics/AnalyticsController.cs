using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public class AnalyticsController : APIController 
	{
		#region Initialization
		protected override void Setup() {}	
		#endregion
		
		#region Wrappers Initialization
		protected override void InitWrappers() 
		{
			base.InitWrappers();
		}
		
		protected override IAPIWrapperInterface GetWrapperForAPI(int wrapperAPI)
		{
			switch (wrapperAPI)
			{
				case APIs.Analytics.Unity:
					return UnityAnalyticsWrapper.Instance;
				default:
					return null;
			}
		}
		#endregion

		#region Controls
		public void LogEvent(string eventName, Dictionary<string, object> parameters)
		{
			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				wrapper.LogEvent(eventName, parameters); 
			}
		}

		public void LogRateApp(bool accepted)
		{
			LogEvent("Rate App", new System.Collections.Generic.Dictionary<string, object>
			{
				{ "accepted", accepted }
			});
		}
		#endregion

		#region Audio
		public void DisableMusic() 
		{ 
			ChangeAudioStatus("Music", false);
		}

		public void EnableMusic() 
		{ 
			ChangeAudioStatus("Music", true);
		}

		public void DisableSounds() 
		{ 
			ChangeAudioStatus("Sound", false);
		}

		public void EnableSounds() 
		{ 
			ChangeAudioStatus("Sound", true);
		}

		private void ChangeAudioStatus(string channel, bool status)
		{
			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				wrapper.LogEvent(channel, new Dictionary<string, object>{
					{ "status", status ? "on" : "off" }
				}); 
			}
		}
		#endregion
		
		#region Localisation
		public void LogSystemLanguage(string languageCode) 
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;
			
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("code", languageCode.ToLower());

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
				wrapper.LogEvent("Localisation - System Language", parameters);
		}
		
		public void LogLanguageChange(string languageCode) 
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;
			
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("code", languageCode.ToLower());

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
				wrapper.LogEvent("Localisation - Change Language", parameters);
		}
		#endregion
		
		#region Gameplay
		public void LogUnlockAchievement(string achievement) 
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				wrapper.LogEvent("Achievement Unlocked", new Dictionary<string, object>
				{
					{ "achievement", achievement }					
				});
			}
		}
		#endregion
	}
}
