using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Events;
using Zedarus.ToolKit.Settings;

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
		public void LogEvent(string eventName)
		{
			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				wrapper.LogEvent(eventName); 
			}
		}

		public void LogEvent(string eventName, Dictionary<string, object> parameters)
		{
			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				wrapper.LogEvent(eventName, parameters); 
			}
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

		#region Event Listeners
		protected override void CreateEventListeners()
		{
			base.CreateEventListeners();
			EventManager.AddListener<string>(IDs.Events.SetLanguage, OnChangeLanguage);
		}

		protected override void RemoveEventListeners()
		{
			base.RemoveEventListeners();
			EventManager.RemoveListener<string>(IDs.Events.SetLanguage, OnChangeLanguage);
		}
		#endregion

		#region Event Handlers
		private void OnChangeLanguage(string language)
		{
			Debug.Log("On change language: " + language);
			LogEvent("Usage - Language", new System.Collections.Generic.Dictionary<string, object>
			{
				{ "language", language }
			});
		}
		#endregion
	}
}
