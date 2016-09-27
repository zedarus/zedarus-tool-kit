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
	public class FreeRemoveAds : ExtentionUIPopup
	{
		#region Properties
		private APISettingsData _gameData;
		private APIState _playerData;
		private string _videoAdID = null;
		private bool _rewardReceived = false;
		private UIManager _ui;
		#endregion

		#region Settings
		private const int BUTTON_FREE = 30;
		private const int BUTTON_PAID = 40;
		private const int BUTTON_CANCEL = 50;
		private const int BUTTON_SUCCESS = 60;
		private const int BUTTON_FAILURE = 70;
		private const int MESSAGE_SUCCESS = 80;
		private const int MESSAGE_FAILURE = 90;
		#endregion

		#region Init
		public FreeRemoveAds(GameData gameData, PlayerData playerData, LocalisationManager localisation, APIManager apiManager, string videoAdID, 
			string genericPopupID, object popupHeader, object popupMessage,
			object freeRemoveButtonLabel, object paidRemoveButtonLabel, object cancelButtonLabel,
			object freeAdsRemoveSuccessMessage, object freeAdsRemoveSuccessButton,
			object freeAdsRemoveFailureMessage, object freeAdsRemoveFailureButton,
			int freeRemoveButtonColorID = 0, int paidRemoveButtonColorID = 0, int cancelButtonColorID = 0) : base(apiManager, localisation, genericPopupID, popupHeader, popupMessage)
		{
			_gameData = gameData.APISettings;
			_playerData = playerData.APIState;
			_videoAdID = videoAdID;

			CreateButtonKeys(BUTTON_FREE, freeRemoveButtonLabel, freeRemoveButtonColorID);
			CreateButtonKeys(BUTTON_PAID, paidRemoveButtonLabel, paidRemoveButtonColorID);
			CreateButtonKeys(BUTTON_CANCEL, cancelButtonLabel, cancelButtonColorID);
			CreateButtonKeys(BUTTON_SUCCESS, freeAdsRemoveSuccessButton, 0);
			CreateButtonKeys(BUTTON_FAILURE, freeAdsRemoveFailureButton, 0);

			CreateLocalisationKey(MESSAGE_SUCCESS, freeAdsRemoveSuccessMessage);
			CreateLocalisationKey(MESSAGE_FAILURE, freeAdsRemoveFailureMessage);
		}
		#endregion

		#region Controls
		public bool DisplayPopup(UIManager uiManager, GameObject adsBlock) 
		{
			_ui = uiManager;
			AssignAdBlock(adsBlock);

			string header = Localise(POPUP_HEADER);
			string message = Localise(POPUP_MESSAGE);

			if (header != null)
			{
				header = string.Format(header, _gameData.FreeAdsRemovalDurationHours);
			}

			if (message != null)
			{
				message = string.Format(message, _gameData.FreeAdsRemovalDurationHours);
			}

			DisplayPopup(uiManager, header, message,
				CreateButton(string.Format(Localise(BUTTON_FREE), _gameData.FreeAdsRemovalDurationHours), OnFreeRemoveClick, BUTTON_FREE),
				CreateButton(BUTTON_PAID, OnPaidRemoveClick, BUTTON_PAID),
				CreateButton(BUTTON_CANCEL, OnCancelClick, BUTTON_CANCEL)
			);

			return true;
		}

		public string GetTimeRemaining()
		{
			if (API.Ads.FreeAdsRemovalEnabled)
			{
				int minutes = API.Ads.FreeAdsRemovalMinutesRemaining;
				int hours = Mathf.FloorToInt(minutes / 60);
				minutes = minutes - hours * 60;
				return string.Format("{0:D2}:{1:D2}", hours, minutes);
			}
			else
			{
				return string.Empty;
			}
		}
		#endregion

		#region Analytics
		protected override string EventName
		{
			get { return "Remove Ads Popup"; }
		}
		#endregion

		#region Helpers
		#endregion

		#region UI Callbacks
		private void OnFreeRemoveClick()
		{
			LogAnalytics("free");
			_rewardReceived = false;
			ActivateAdBlock();
			API.Ads.ShowRewardedVideo(_videoAdID, OnRewardVideoClose, OnRewardVideoReward, 0);
		}

		private void OnPaidRemoveClick()
		{
			LogAnalytics("paid");
			if (_gameData.AdsEnabled)
			{
				API.Store.Purchase(_gameData.RemoveAdsIAPID, null);
			}
		}

		private void OnCancelClick()
		{
			LogAnalytics("no");
		}

		private void OnRewardVideoClose()
		{
			Zedarus.ToolKit.DelayedCall.Create(WaitAndCheckForReward, 3f);
		}

		private void OnRewardVideoReward(int productID)
		{
			DeactivateAdBlock();
			LogAnalytics("reward - success");
			_rewardReceived = true;
			_playerData.RegisterFreeAdsRemoval();
			Zedarus.ToolKit.Events.EventManager.SendEvent(Zedarus.ToolKit.Settings.IDs.Events.AdsDisabled);

			if (_ui != null)
			{
				DisplayPopup(_ui, null, string.Format(Localise(MESSAGE_SUCCESS), _gameData.FreeAdsRemovalDurationHours), CreateButton(BUTTON_SUCCESS, null, BUTTON_SUCCESS));
			}
		}

		private void WaitAndCheckForReward()
		{
			if (!_rewardReceived)
			{
				DeactivateAdBlock();
				LogAnalytics("reward - failure");

				if (_ui != null)
				{
					DisplayPopup(_ui, null, Localise(MESSAGE_FAILURE), CreateButton(BUTTON_FAILURE, null, BUTTON_FAILURE));
				}
			}
		}
		#endregion
	}
}
