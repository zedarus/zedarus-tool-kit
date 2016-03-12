using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

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
		LanguageSelection,
		Tutorial
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
		FreePack,
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
		RemoveAds,
		CoinsShop,
		Undo,
		NotEnoughCoins,
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
		Message,
		GameCenter,
		iCloud,
		iCloudExistingData,
		QuitGame,
		AppOfTheDayPromo
	}
	
	public class AnalyticsController : APIController 
	{
		#region Initialization
		protected override void Setup() {}	
		#endregion

		#region Properties
		private LogScreens _previousScreen = LogScreens.Options;
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
				case APIs.UnityAnalytics:
					return UnityAnalyticsWrapper.Instance;
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

		public void LogLevelPackUnlockRequest(int levelPackID)
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;

			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("levelpackID", levelPackID.ToString());

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				string eventName = "Monetization - LevelPackUnlock - Request";
				wrapper.LogEvent(eventName, parameters);
			}
		}

		public void LogLevelPackUnlock(int levelPackID, int price, bool success)
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;

			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("levelpackID", levelPackID.ToString());
			parameters.Add("price", price.ToString());
			parameters.Add("success", success.ToString());

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				string eventName = "Monetization - LevelPackUnlock - Unlock";
				wrapper.LogEvent(eventName, parameters);
			}
		}
		
		#region User Interface
		public void LogScreen(LogScreens screen) 
		{
			if (_previousScreen == screen)
				return;

			if (Wrappers == null || Wrappers.Count < 1)
				return;

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				string eventName = "UI - " + GetScreenName(screen) + " - Display";
				wrapper.LogEvent(eventName);
			}

			_previousScreen = screen;
		}

		public void LogPromo(string promoName)
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
				wrapper.LogEvent("Promo - " + promoName);
		}
		
		private void LogButton(LogScreens screen, LogButtons button) 
		{
			Dictionary<string,object> noParams = null;
			LogButton(screen, button, noParams);	
		}
		
		private void LogButton(LogScreens screen, LogButtons button, params string[] parameters) 
		{	
			if (parameters.Length % 2 == 0 && parameters.Length > 0)
			{
				Dictionary<string,object> filteredParameters = new Dictionary<string, object>();
				for (int i = 0; i < parameters.Length / 2; i += 2)
				{
					filteredParameters.Add(parameters[i], parameters[i+1]);
				}
				LogButton(screen, button, filteredParameters);
			}
			else
				Debug.LogWarning("Incorrect number of parameters for event");
		}
		
		private void LogButton(LogScreens screen, LogButtons button, Dictionary<string,object> parameters) 
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;
			
			string eventName = "UI - " + GetScreenName(screen) + " - Button - " + GetButtonName(button);

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				if (parameters != null)
					wrapper.LogEvent(eventName, parameters);
				else
					wrapper.LogEvent(eventName);
			}
		}
		#endregion
		
		#region User Interface
		public void LogShopPopup(LogPopups popup, LogScreens location, LogButtons button)
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;

			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("location", GetScreenName(location));
			parameters.Add("button", GetButtonName(button));

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				string eventName = "UI - Popup - " + GetPopupName(popup) + " - Display";
				wrapper.LogEvent(eventName, parameters);
			}
		}

		public void LogShopPopupButton(LogButtons button)
		{
			LogPopupButton(LogPopups.Shop, button);
		}

		public void LogRateMePopup(LogScreens location)
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;

			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("location", GetScreenName(location));

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				string eventName = "UI - Popup - " + GetPopupName(LogPopups.Rate) + " - Display";
				wrapper.LogEvent(eventName, parameters);
			}
		}

		public void LogRateMePopupButton(LogButtons button)
		{
			LogPopupButton(LogPopups.Rate, button);
		}

		private void LogPopup(LogPopups popup, string message = null) 
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;

			Dictionary<string, object> parameters = null;

			if (message != null)
			{
				parameters = new Dictionary<string, object>();
				parameters.Add("message", message);
			}

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				string eventName = "UI - Popup - " + GetPopupName(popup) + " - Display";
				if (parameters != null)
					wrapper.LogEvent(eventName, parameters);
				else
					wrapper.LogEvent(eventName);
			}
		}
		
		private void LogPopupButton(LogPopups popup, LogButtons button) 
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				string eventName = "UI - Popup - " + GetPopupName(popup) + " - Button - " + GetButtonName(button);
				wrapper.LogEvent(eventName);
			}
		}
		#endregion
		
		#region Sync
		public void LogDataSyncStatusChange(bool status) 
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				string eventName = "Sync - iCloud - " + (status ? "On" : "Off");
				wrapper.LogEvent(eventName);
			}
		}
		#endregion
		
		#region External Links
		public void LogExternalLink(string link, bool inBrowser) 
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;
			
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("url", link);
			parameters.Add("in_browser", (inBrowser ? "Yes":"No"));

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
				wrapper.LogEvent("Open External Link", parameters);
		}
		#endregion
		
		#region Audio
		public void DisableMusic() 
		{ 
			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
				wrapper.LogEvent("Disable Music"); 
		}

		public void EnableMusic() 
		{ 
			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
				wrapper.LogEvent("Enable Music"); 
		}

		public void DisableSounds() 
		{ 
			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
				wrapper.LogEvent("Disable Sounds"); 
		}

		public void EnableSounds() 
		{ 
			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
				wrapper.LogEvent("Enable Sounds"); 
		}

		public void LogIPodMusicStatus(bool playing, bool onAppStart) 
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;
			
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("music_playing", (playing ? "Yes":"No"));
			parameters.Add("on_app_start", (onAppStart ? "Yes":"No"));

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
				wrapper.LogEvent("iPod Music Status", parameters);
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
		
		#region IAPs
		public void LogPurchase(bool result, string item, decimal price, string currency) 
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;

			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("item", item);

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
			{
				if (result)
					wrapper.LogPurchase(item, price, currency);
				else
					wrapper.LogEvent("IAPs - Purchase Failed", parameters);
			}
		}
		#endregion
		
		#region Social
		public void LogLevelShare(string media) 
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;
			
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("media", media);

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
				wrapper.LogEvent("Social - Share Level Screenshot", parameters);
		}
		#endregion
		
		#region Gameplay
		public void LogUnlockAchievement(int achievementID) 
		{
			if (Wrappers == null || Wrappers.Count < 1)
				return;

			/*AchievementData achievement = GameDataManager.Instance.GetAchievementWithID(achievementID);
			int gameMode = PlayerDataManager.Instance.GetCurrentGameModeID();
			float playtime = PlayerDataManager.Instance.GetTotalPlaytime(gameMode);
			int levelsCompleted = PlayerDataManager.Instance.GetNumberOfLevelsCompletedInGameModeWithID(gameMode);

			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("achievement_id", achievement.id.ToString());
			parameters.Add("achievement_name", achievement.GameCenterIosID);
			parameters.Add("playtime_seconds", Mathf.CeilToInt(playtime));
			parameters.Add("levels_completed", levelsCompleted);

			foreach (IAnalyticsWrapperInterface wrapper in Wrappers)
				wrapper.LogEvent("Game - Unlock Achievement", parameters);*/
		}
		#endregion
	}
}
