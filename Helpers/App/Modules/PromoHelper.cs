using System;
using System.Collections.Generic;
using UnityEngine;
using Zedarus.ToolKit.API;
using Zedarus.ToolKit.Settings;
using Zedarus.ToolKit.Events;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Player;

namespace Zedarus.ToolKit.Helpers.Modules
{
	public class PromoHelper : AppHelperBase
	{
		#region Init
		public PromoHelper(GameData gameDataRef, APIManager api) : base(gameDataRef, api) {}
		#endregion

		#region Controls
		public void SendContactsEmail()
		{
			Application.OpenURL("mailto:" + GameData.Settings.ContactEmail + "?subject=" + EscapeURL(GameData.Settings.ContactEmailSubject));
		}

		public bool OnMoreLevelsPress()
		{
			API.Analytics.LogEvent("Promo - Open More Levels");
			if (GameData.Settings.UseLinkForMoreLevels)
			{
				Application.OpenURL(GameData.Settings.MoreLevelsURL);
				return true;
			}
			else
			{
				// Display rate me popup here
				return false;
			}
		}

		public void OnMoreGamesPress()
		{
			API.Analytics.LogEvent("Promo - Open More Games");
			if (GameData.Settings.UseAdsForMoreGames)
			{
				// TODO: use event here temporarily, only because we can't access AppController directly from here because of generics right now
				EventManager.SendEvent<string>(IDs.Events.DisplayAdPlacement, GameData.Settings.MoreGamesAdPlacement);
			}
			else
			{
				Application.OpenURL(GameData.Settings.MoreGamesAdPlacement);
			}
		}

		public void OnFacebookButtonPress(string placement)
		{
			API.Analytics.LogEvent("Promo - Open Facebook Page", new Dictionary<string, object> {
				{ "placement", placement }
			});
			Application.OpenURL(GameData.Settings.FacebookURL);
		}

		public void OpenRateAppPage()
		{
			API.Analytics.LogEvent("Promo - Open AppStore Rate Link");
			// TODO: log to analytics here
			API.Analytics.LogRateApp(true);
			Application.OpenURL(GameData.RateMePopup.CurrentPlatformURL);
		}
		#endregion

		#region Helpers
		private string EscapeURL(string url)
		{
			return WWW.EscapeURL(url).Replace("+","%20");
		}
		#endregion
	}
}

