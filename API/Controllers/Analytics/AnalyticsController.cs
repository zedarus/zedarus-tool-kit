using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using Zedarus.Traffico.Data.GameData;
using Zedarus.Traffico.Data.GameData.Models;
using Zedarus.Traffico.Data.PlayerData;
using Zedarus.Traffico.Settings;

namespace Zedarus.ToolKit.API
{
	public enum LogScreens
	{
		Menu,
		Options,
		Credits,
		MoreGames,
		LevelSelection,
		Game,
		GamePause,
		GameCompletionScreen,
		LanguageSelection
	}
	
	public enum LogButtons
	{
		Play,
		Score,
		Options,
		Sync,
		Copyrights,
		Back,
		Sound,
		Music,
		LanguageSelection,
		Language,
		GameCenter,
		iCloud,
		Credits,
		ResetProgress,
		Contact,
		Support,
		MoreGamesLink,
		CoinsPanel,
		ShareGame,
		MoreGames,
		MoreLevels,
		LevelPack,
		Level,
		Pause,
		Hint,
		Solve,
		NextLevel,
		ShareLevel,
		Wiki,
		Restart,
		Continue,
		Exit,
		RateGame,
		SmallPack,
		MediumPack,
		BigPack,
		Cancel,
		Yes,
		No,
		GetMoreCoins,
		OK,
		RateNow,
		RateLater,
		NeverAsk,
		GiftApp,
		Facebook,
		Twitter,
		Email,
		Learn,
		RestorePurchases,
		RemoveAds
	}
	
	public enum LogPopups
	{
		Shop,
		Hint,
		NotEnoughCoins,
		Processing,
		QuitLevel,
		Rate,
		ResetGame,
		ShareApp,
		ShareLevel,
		SharingResult,
		Solve,
		TransactionResult,
		GameCenter,
		iCloud,
		iCloudExistingData,
		QuitGame,
		AppOfTheDayPromo
	}
	
	public class AnalyticsController : APIController 
	{
		#region Initialization
		public AnalyticsController(MultipleAPIUseMode useMode, params APIs[] values) : base(useMode, values) {}
		protected override void Setup() {}	
		#endregion
		
		#region Wrappers Initialization
		protected override void InitWrappers() 
		{
			base.InitWrappers();
		}
		
		protected override IAPIWrapperInterface GetWrapperForAPI(APIs wrapperAPI)
		{
			switch (wrapperAPI)
			{
				#if API_FLURRY_P31
				case APIs.Flurry:
					return FlurryWrapper.Instance;
				#endif
				default:
					return null;
			}
		}
		#endregion
		
		private string GetButtonName(LogButtons button)
		{
			return Regex.Replace(button.ToString(), "(\\B[A-Z])", " $1");
		}
		
		private string GetScreenName(LogScreens screen)
		{
			return Regex.Replace(screen.ToString(), "(\\B[A-Z])", " $1");
		}
		
		private string GetPopupName(LogPopups popup)
		{
			return Regex.Replace(popup.ToString(), "(\\B[A-Z])", " $1");
		}
		
		#region User Interface
		public void LogScreen(LogScreens screen) 
		{
			if (Wrapper == null)
				return;
			
			string eventName = "UI - " + GetScreenName(screen) + " - Display";
			Wrapper.LogEvent(eventName);
		}

		public void LogPromo(string promoName)
		{
			if (Wrapper == null)
				return;

			Wrapper.LogEvent("Promo - " + promoName);
		}
		
		public void LogButton(LogScreens screen, LogButtons button) 
		{
			Dictionary<string,string> noParams = null;
			LogButton(screen, button, noParams);	
		}
		
		public void LogButton(LogScreens screen, LogButtons button, params string[] parameters) 
		{	
			if (parameters.Length % 2 == 0 && parameters.Length > 0)
			{
				Dictionary<string,string> filteredParameters = new Dictionary<string, string>();
				for (int i = 0; i < parameters.Length / 2; i += 2)
				{
					filteredParameters.Add(parameters[i], parameters[i+1]);
				}
				LogButton(screen, button, filteredParameters);
			}
			else
				Debug.LogWarning("Incorrect number of parameters for event");
		}
		
		public void LogButton(LogScreens screen, LogButtons button, Dictionary<string,string> parameters) 
		{
			if (Wrapper == null)
				return;
			
			string eventName = "UI - " + GetScreenName(screen) + " - Button - " + GetButtonName(button);
			
			if (parameters != null)
				Wrapper.LogEvent(eventName, parameters);
			else
				Wrapper.LogEvent(eventName);
		}
		#endregion
		
		#region User Interface
		public void LogPopup(LogPopups popup) 
		{
			if (Wrapper == null)
				return;
			
			string eventName = "UI - Popup - " + GetPopupName(popup) + " - Display";
			Wrapper.LogEvent(eventName);
		}
		
		public void LogPopupButton(LogPopups popup, LogButtons button) 
		{
			if (Wrapper == null)
				return;
			
			string eventName = "UI - Popup - " + GetPopupName(popup) + " - Button - " + GetButtonName(button);
			Wrapper.LogEvent(eventName);
		}
		#endregion
		
		#region Sync
		public void LogDataSyncStatusChange(bool status) 
		{
			if (Wrapper == null)
				return;
			
			string eventName = "Sync - iCloud - " + (status ? "On" : "Off");
			Wrapper.LogEvent(eventName);
		}
		
		public void LogDataSyncEvent() 
		{
			if (Wrapper == null)
				return;
			
			string eventName = "Sync - iCloud - Data Update";
			Wrapper.LogEvent(eventName);
		}
		#endregion
		
		#region External Links
		public void LogExternalLink(string link, bool inBrowser) 
		{
			if (Wrapper == null) return;
			
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("url", link);
			parameters.Add("in_browser", (inBrowser ? "Yes":"No"));
			if (Wrapper != null) Wrapper.LogEvent("Open External Link", parameters);
		}
		#endregion
		
		#region Audio
		public void DisableMusic() { if (Wrapper != null) Wrapper.LogEvent("Disable Music"); }
		public void EnableMusic() { if (Wrapper != null) Wrapper.LogEvent("Enable Music"); }
		public void DisableSounds() { if (Wrapper != null) Wrapper.LogEvent("Disable Sounds"); }
		public void EnableSounds() { if (Wrapper != null) Wrapper.LogEvent("Enable Sounds"); }
		public void LogIPodMusicStatus(bool playing, bool onAppStart) 
		{
			if (Wrapper == null) return;
			
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("music_playing", (playing ? "Yes":"No"));
			parameters.Add("on_app_start", (onAppStart ? "Yes":"No"));
			if (Wrapper != null) Wrapper.LogEvent("iPod Music Status", parameters);
		}
		#endregion
		
		#region Localisation
		public void LogSystemLanguage(string languageCode) 
		{
			if (Wrapper == null) return;
			
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("code", languageCode.ToLower());
			if (Wrapper != null) Wrapper.LogEvent("Localisation - System Language", parameters);
		}
		
		public void LogLanguageChange(string languageCode) 
		{
			if (Wrapper == null) return;
			
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("code", languageCode.ToLower());
			if (Wrapper != null) Wrapper.LogEvent("Localisation - Change Language", parameters);
		}
		#endregion
		
		#region Performance
		public void LogBackgroundGenerationTime(float seconds, string device, bool initial) 
		{
			if (Wrapper == null) return;
			
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("time_seconds", seconds.ToString());
			parameters.Add("device", device);
			parameters.Add("inital", (initial ? "Yes":"No"));
			if (Wrapper != null) Wrapper.LogEvent("Performance - Background Generation", parameters);
		}
		
		public void LogLevelGenerationTime(float seconds, string device) 
		{
			if (Wrapper == null) return;
			
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("time_seconds", seconds.ToString());
			parameters.Add("device", device);
			if (Wrapper != null) Wrapper.LogEvent("Performance - Level Generation", parameters);
		}
		#endregion
		
		#region IAPs
		public void LogPurchase(bool result, string item) 
		{
			if (Wrapper == null) return;
			
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("item", item);
			if (result)
				Wrapper.LogEvent("IAPs - Purchase Successfull", parameters);
			else
				Wrapper.LogEvent("IAPs - Purchase Failed", parameters);
		}
		#endregion
		
		#region Social
		public void LogLevelShare(string media) 
		{
			if (Wrapper == null) return;
			
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("media", media);
			Wrapper.LogEvent("Social - Share Level Started", parameters);
		}
		
		public void LogGameShare(string media) 
		{
			if (Wrapper == null) return;
			
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("media", media);
			Wrapper.LogEvent("Social - Share Game Started", parameters);
		}
		
		public void LogGiftApp() 
		{
			if (Wrapper != null) Wrapper.LogEvent("Social - Start Gift Process");
		}
		#endregion
		
		#region Gameplay
		public void LogUnlockAchievement(int achievementID) 
		{
			AchievementData achievement = GameDataManager.Instance.GetAchievementWithID(achievementID);
			int gameMode = PlayerDataManager.Instance.GetCurrentGameModeID();
			//float playtime = PlayerDataManager.Instance.GetTotalPlaytime(gameMode);
			//int levelsCompleted = PlayerDataManager.Instance.GetNumberOfLevelsCompletedInGameModeWithID(gameMode);
			
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("achievement_id", achievement.id.ToString());
			parameters.Add("achievement_name", achievement.GameCenterIosID);
			parameters.Add("game_mode_id", gameMode.ToString());
			//parameters.Add("playtime_seconds", Mathf.CeilToInt(playtime).ToString());
			//parameters.Add("levels_completed", levelsCompleted.ToString());
			
			if (Wrapper != null) Wrapper.LogEvent("Game - Unlock Achievement", parameters);
		}
		#endregion
		
		#region Getters
		protected IAnalyticsWrapperInterface Wrapper
		{
			get { return (IAnalyticsWrapperInterface)CurrentWrapperBase; }
		}
		#endregion
	}
}
