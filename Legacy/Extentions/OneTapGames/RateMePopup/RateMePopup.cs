using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.UI;
using Zedarus.ToolKit.API;
using Zedarus.ToolKit.Localisation;

namespace Zedarus.ToolKit.Extentions.OneTapGames.RateMePopup
{
	public class RateMePopup : ExtentionUIPopup
	{
		#region Properties
		private RateMePopupData _data;
		private RateMePopupModel _playerData;
		private Wallet _wallet;
		public System.Action _callback = null;
		#endregion

		#region Settings
		private const int BUTTON_REWARD = 101;
		private const int BUTTON_YES = 102;
		private const int BUTTON_NO = 103;

		private const int MESSAGE_REWARD = 104;
		private const int MESSAGE_RATE = 105;
		#endregion

		#region Init
		public RateMePopup(GameData gameData, PlayerData playerData, APIManager apiManager, Wallet wallet, LocalisationManager localisation, string genericPopupID, 
			object rateMessage, object rateConfirmButtonLabel, object rateDeclineButtonLabel, 
			object rewardMessage, object rewardConfirmationButtonLabel, 
			int rateConfirmButtonColorID = 0, int rateDeclineButtonColorID = 0, int rewardConfirmationButtonColorID = 0) : base(apiManager, localisation, genericPopupID, null, null)
		{
			_data = gameData.Get<RateMePopupData>().First;
			_playerData = playerData.GetModel<RateMePopupModel>();

			if (_data == null)
			{
				throw new UnityException("No RateMePopupData found in game data");
			}

			if (_playerData == null)
			{
				throw new UnityException("No RateMePopupModel found in player data");
			}

			_wallet = wallet;

			CreateButtonKeys(BUTTON_YES, rateConfirmButtonLabel, rateConfirmButtonColorID);
			CreateButtonKeys(BUTTON_NO, rateDeclineButtonLabel, rateDeclineButtonColorID);
			CreateButtonKeys(BUTTON_REWARD, rewardConfirmationButtonLabel, rewardConfirmationButtonColorID);

			CreateLocalisationKey(MESSAGE_REWARD, rewardMessage);
			CreateLocalisationKey(MESSAGE_RATE, rateMessage);
		}
		#endregion

		#region Controls
		public bool DisplayPopup(UIManager uiManager, System.Action finishCallback, bool betweenLevel, bool skipReward)
		{
			bool display = true;

			if (betweenLevel)
			{
				_playerData.IterateEvent();
				display = _playerData.CanDisplayBetweenLevels(_data.EventsDelay, _data.DisplayOnce);
			}

			if (display)
			{
				_callback = finishCallback;

				if (_data.RewardFirst && !skipReward)
				{
					_wallet.Deposit(_data.Reward);
					DisplayPopup(uiManager, null, string.Format(Localise(MESSAGE_REWARD), _data.Reward), CreateButton(BUTTON_REWARD, null));
				}

				DisplayPopup(uiManager, null, Localise(MESSAGE_RATE), 
					CreateButton(BUTTON_YES, OnAgreeToRate),
					CreateButton(BUTTON_NO, OnDeclineToRate)
				);

				return true;
			}
			else
			{
				return false;
			}
		}

		private void OpenRateAppPage()
		{
			LogAnalytics("open store link");
			Application.OpenURL(_data.CurrentPlatformURL);
		}
		#endregion

		#region UI Callbacks
		private void OnAgreeToRate()
		{
			LogAnalytics("rate");
			OpenRateAppPage();
			ExecuteCallback();
		}

		private void OnDeclineToRate()
		{
			LogAnalytics("decline");
			ExecuteCallback();
		}
		#endregion

		#region Helpers
		private void ExecuteCallback()
		{
			if (_callback != null)
			{
				_callback();
				_callback = null;
			}
		}
		#endregion

		#region Analytics
		protected override string EventName
		{
			get { return "Rate Me Popup"; }
		}
		#endregion
	}
}
