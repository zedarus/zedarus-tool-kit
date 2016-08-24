using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.UI;
using Zedarus.ToolKit.API;
using Zedarus.ToolKit.Localisation;

namespace Zedarus.ToolKit.Extentions.OneTapGames.FreeRemoveAds
{
	public class FreeRemoveAds : Extention
	{
		#region Properties
		private APIManager _api;
		private APISettingsData _gameData;
		private APIState _playerData;
		private string _videoAdID = null;
		private string _successMessage = null;
		private string _successButton = null;
		private string _failureMessage = null;
		private string _failureButton = null;
		private UIManager _ui;
		private string _genericPopupID;
		#endregion

		#region Events
//		public event System.Action DoubleCoinsCancel;
		#endregion

		#region Init
		public FreeRemoveAds(GameData gameData, PlayerData playerData, APIManager apiManager, string videoAdID) : base()
		{
			_gameData = gameData.APISettings;
			_playerData = playerData.APIState;
			_api = apiManager;
			_videoAdID = videoAdID;
		}
		#endregion

		#region Controls
		public bool DisplayPopup(UIManager uiManager, string genericPopupID, 
			string header, string message, string freeRemoveButtonLabel, string paidRemoveButtonLabel, string cancelButtonLabel,
			string freeAdsRemoveSuccessMessage, string freeAdsRemoveSuccessButton,
			string freeAdsRemoveFailureMessage, string freeAdsRemoveFailureButton,
			int freeRemoveButtonColorID = 0, int paidRemoveButtonColorID = 0, int cancelButtonColorID = 0) 
		{
			List<Zedarus.ToolKit.UI.UIGenericPopupButtonData> buttons = new List<Zedarus.ToolKit.UI.UIGenericPopupButtonData>();

			buttons.Add(new Zedarus.ToolKit.UI.UIGenericPopupButtonData(
				string.Format(freeRemoveButtonLabel, _gameData.FreeAdsRemovalDurationHours), OnFreeRemoveClick, freeRemoveButtonColorID
			));

			buttons.Add(new Zedarus.ToolKit.UI.UIGenericPopupButtonData(
				paidRemoveButtonLabel, OnPaidRemoveClick, paidRemoveButtonColorID
			));

			buttons.Add(new Zedarus.ToolKit.UI.UIGenericPopupButtonData(
				cancelButtonLabel, OnCancelClick, cancelButtonColorID
			));

			if (header != null)
			{
				header = string.Format(header, _gameData.FreeAdsRemovalDurationHours);
			}

			if (message != null)
			{
				message = string.Format(message, _gameData.FreeAdsRemovalDurationHours);
			}

			_successMessage = freeAdsRemoveSuccessMessage;
			_successButton = freeAdsRemoveSuccessButton;

			_failureMessage = freeAdsRemoveFailureMessage;
			_failureButton = freeAdsRemoveFailureButton;

			if (_successMessage != null)
			{
				_successMessage = string.Format(_successMessage, _gameData.FreeAdsRemovalDurationHours);
			}

			_ui = uiManager;
			_genericPopupID = genericPopupID;

			_ui.OpenPopup(genericPopupID, new Zedarus.ToolKit.UI.UIGenericPopupData(
				header, message, buttons.ToArray()
			));

			return true;
		}
		#endregion

		#region Helpers
		private void LogAnalytics(string action)
		{
			_api.Analytics.LogEvent("Monetisation - Remove Ads Popup", new Dictionary<string, object> {
				{ "action", action }
			});
		}
		#endregion

		#region Getters
		#endregion

		#region UI Callbacks
		private void OnFreeRemoveClick()
		{
			LogAnalytics("free");
			_api.Ads.ShowRewardedVideo(_videoAdID, OnRewardVideoClose, OnRewardVideoReward, 0);
		}

		private void OnPaidRemoveClick()
		{
			LogAnalytics("paid");
			if (_gameData.AdsEnabled)
			{
				_api.Store.Purchase(_gameData.RemoveAdsIAPID, null);
			}
		}

		private void OnCancelClick()
		{
			LogAnalytics("no");
		}

		private void OnRewardVideoClose()
		{
			Zedarus.ToolKit.DelayedCall.Create(WaitAndCheckForReward, 1f);
		}

		private void OnRewardVideoReward(int productID)
		{
			_playerData.RegisterFreeAdsRemoval();
			Zedarus.ToolKit.Events.EventManager.SendEvent(Zedarus.ToolKit.Settings.IDs.Events.AdsDisabled);

			if (_ui != null)
			{
				_ui.OpenPopup(_genericPopupID, new Zedarus.ToolKit.UI.UIGenericPopupData(
					null, _successMessage, new Zedarus.ToolKit.UI.UIGenericPopupButtonData(_successButton)
				));
			}
		}

		private void WaitAndCheckForReward()
		{
			if (_ui != null)
			{
				_ui.OpenPopup(_genericPopupID, new Zedarus.ToolKit.UI.UIGenericPopupData(
					null, _failureMessage, new Zedarus.ToolKit.UI.UIGenericPopupButtonData(_failureButton)
				));
			}
		}
		#endregion
	}
}
