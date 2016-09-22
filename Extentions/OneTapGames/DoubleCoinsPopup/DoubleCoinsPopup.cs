using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.UI;
using Zedarus.ToolKit.API;
using Zedarus.ToolKit.Localisation;

namespace Zedarus.ToolKit.Extentions.OneTapGames.DoubleCoinsPopup
{
	public class DoubleCoinsPopup : ExtentionUIPopup
	{
		#region Properties
		private DoubleCoinsPopupData _data;
		private Wallet _wallet;
		private bool _newSession = false;
		private bool _openSession = false;
		private string _videoAdID = null;
		private int _sessions = 0;
		private int _additionalCoins = 0;
		private System.Action<bool> _callback;
		#endregion

		#region Settings
		private const int BUTTON_AGREE = 10;
		private const int BUTTON_CANCEL = 12;
		#endregion

		#region Init
		public DoubleCoinsPopup(GameData gameData, APIManager apiManager, Wallet wallet, LocalisationManager localisation, string videoAdID, string genericPopupID, 
			object popupHeaderStringID, object popupMessageStringID,
			object agreeButtonLocalisationID, object cancelButtonLocalisationID, int agreeButtonColorID = 0, int cancelButtonColorID = 0) : base(apiManager, localisation, genericPopupID, popupHeaderStringID, popupMessageStringID)
		{
			_data = gameData.Get<DoubleCoinsPopupData>().First;

			if (_data == null)
			{
				throw new UnityException("No DoubleCoinsPopupData found in game data");
			}

			_wallet = wallet;
			_videoAdID = videoAdID;
			_sessions = 0;

			CreateButtonKeys(BUTTON_AGREE, agreeButtonLocalisationID, agreeButtonColorID);
			CreateButtonKeys(BUTTON_CANCEL, cancelButtonLocalisationID, cancelButtonColorID);
		}
		#endregion

		#region Controls
		internal override void RegisterSessionStart()
		{
			_newSession = true;
			_openSession = true;
		}

		internal override void RegisterSessionEnd()
		{
			if (_openSession)
			{
				_openSession = false;
				_sessions++;
			}
		}

		public bool DisplayPopup(UIManager uiManager, int coinsEarned, System.Action<bool> callback)
		{
			if (CanUse(coinsEarned) && _data.Multiplier > 1)
			{
				int multiplier = _data.Multiplier - 1;

				if (multiplier < 0)
				{
					multiplier = 0;
				}

				_additionalCoins = coinsEarned * multiplier;

				string header = Localise(POPUP_HEADER);
				string message = Localise(POPUP_MESSAGE);

				if (header != null)
				{
					header = string.Format(header, coinsEarned);
				}

				if (message != null)
				{
					message = string.Format(message, coinsEarned);
				}

				DisplayPopup(uiManager, header, message,
					CreateButton(BUTTON_AGREE, OnDoubleConfirmed, BUTTON_AGREE),
					CreateButton(BUTTON_CANCEL, OnCancel, BUTTON_CANCEL)
				);

				_callback = callback;

				return true;
			}
			else
			{
				_additionalCoins = 0;
				_callback = null;
				return false;
			}
		}

		private bool CanUse(int coinsEarned)
		{
			bool canDisplay = false;

			if (coinsEarned >= _data.MinCoins)
			{
				canDisplay = true;
			}

			int sessions = _sessions - _data.Offset;

			if (sessions < 0)
			{
				sessions = 0;
				canDisplay = false;
			}

			if (sessions % _data.Delay != 0)
			{
				canDisplay = false;
			}

			if (coinsEarned <= 0)
			{
				canDisplay = false;
			}

			if (!_data.Enabled)
			{
				canDisplay = false;
			}

			if (!_newSession)
			{
				canDisplay = false;
			}

			return canDisplay;
		}
		#endregion

		#region Helpers
		private void Use()
		{
			_newSession = false;

			if (_callback != null)
			{
				if (_wallet != null)
				{
					_wallet.Deposit(_additionalCoins);
				}

				_callback(true);
				_additionalCoins = 0;
				_callback = null;
			}
		}

		private void Decline()
		{
			_newSession = false;
			if (_callback != null)
			{
				_callback(false);
				_additionalCoins = 0;
				_callback = null;
			}
		}
		#endregion

		#region Getters
		#endregion

		#region Analytics
		protected override string EventName
		{
			get { return "Double Coins Popup"; }
		}
		#endregion

		#region UI Callbacks
		private void OnDoubleConfirmed()
		{
			LogAnalytics("yes");

			API.Ads.ShowRewardedVideo(_videoAdID, OnSecondChanceRewardVideoClose, OnSecondChanceRewardVideoReward, 0);
		}

		private void OnCancel()
		{
			LogAnalytics("no");

			Decline();
		}

		private void OnSecondChanceRewardVideoClose()
		{
			Zedarus.ToolKit.DelayedCall.Create(WaitAndCheckForReward, 1f);
		}

		private void WaitAndCheckForReward()
		{
			if (_newSession)
			{
				Decline();
			}
		}

		private void OnSecondChanceRewardVideoReward(int productID)
		{
			if (_newSession)
			{
				Use();
			}
		}
		#endregion
	}
}
