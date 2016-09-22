using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.UI;
using Zedarus.ToolKit.API;
using Zedarus.ToolKit.Localisation;

namespace Zedarus.ToolKit.Extentions.OneTapGames.SecondChancePopup
{
	public class SecondChancePopup : ExtentionUIPopup
	{
		#region Properties
		private SecondChancePopupData _data;
		private Wallet _wallet;
		private bool _newSession = false;
		private string _videoAdID = null;
		private int _sessions = 0;
		public System.Action<bool> _callback;
		#endregion

		#region Settings
		private const int BUTTON_FREE = 3;
		private const int BUTTON_PAID = 4;
		private const int BUTTON_CANCEL = 5;
		#endregion

		#region Init
		// TODO: document this
		public SecondChancePopup(GameData gameData, APIManager apiManager, Wallet wallet, LocalisationManager localisation, string videoAdID, string genericPopupID, 
			object popupHeadeStringID, object popupMessageStringID, 
			object freeChanceButtonLabelID, object paidChanceButtonLabelID, object cancelButtonLabelID, 
			int freeChanceButtonColorID = 0, int paidChanceButtonColorID = 0, int cancelButtonColorID = 0) : base(apiManager, localisation, genericPopupID, popupHeadeStringID, popupMessageStringID)
		{
			// TODO: check for null here and throw exception
			_data = gameData.Get<SecondChancePopupData>().First;
			_wallet = wallet;

			_videoAdID = videoAdID;
			_sessions = 0;

			CreateButtonKeys(BUTTON_FREE, freeChanceButtonLabelID, freeChanceButtonColorID);
			CreateButtonKeys(BUTTON_PAID, paidChanceButtonLabelID, paidChanceButtonColorID);
			CreateButtonKeys(BUTTON_CANCEL, cancelButtonLabelID, cancelButtonColorID);
		}
		#endregion

		#region Controls
		public void RegisterSessionStart()
		{
			_newSession = true;
		}

		public void RegisterSessionEnd()
		{
			_sessions++;
		}

		public bool DisplayPopup(UIManager uiManager, int score, System.Action<bool> callback)
		{
			if (CanUseSecondChance(score))
			{
				if (_wallet.Balance >= _data.Price)
				{
					DisplayPopup(uiManager, 
						CreateButton(BUTTON_FREE, OnFreeSecondChanceConfirmed, BUTTON_FREE),
						CreateButton(string.Format(Localise(BUTTON_PAID), _data.Price), OnPaidSecondChanceConfirmed, BUTTON_PAID),
						CreateButton(BUTTON_CANCEL, OnSecondChanceDenied, BUTTON_CANCEL)
					);
				}
				else
				{
					DisplayPopup(uiManager, 
						CreateButton(BUTTON_FREE, OnFreeSecondChanceConfirmed, BUTTON_FREE),
						CreateButton(BUTTON_CANCEL, OnSecondChanceDenied, BUTTON_CANCEL)
					);
				}

				_callback = callback;
				return true;
			}
			else
			{
				_callback = null;
				return false;
			}
		}

		private bool CanUseSecondChance(int score)
		{
			bool canDisplay = true;

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

			if (score >= _data.MandatoryScore)
			{
				canDisplay = true;
			}
				
			if (UsedOrDeclinedThisSession)
			{
				canDisplay = false;
			}

			if (score <= 0)
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
				_callback(true);
				_callback = null;
			}
		}

		private void Decline()
		{
			_newSession = false;
			if (_callback != null)
			{
				_callback(false);
				_callback = null;
			}
		}
		#endregion

		#region Getters
		private bool UsedOrDeclinedThisSession
		{
			get { return !_newSession; }
		}
		#endregion

		#region UI Callbacks
		private void OnFreeSecondChanceConfirmed()
		{
			LogAnalytics("free");
			API.Ads.ShowRewardedVideo(_videoAdID, OnSecondChanceRewardVideoClose, OnSecondChanceRewardVideoReward, 0);
		}

		private void OnPaidSecondChanceConfirmed()
		{
			if (_wallet != null)
			{
				if (_wallet.Withdraw(_data.Price))
				{
					LogAnalytics("paid");
					Use();
				}
			}
		}

		private void OnSecondChanceDenied()
		{
			LogAnalytics("cancel");
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

		#region Analytics
		protected override string EventName
		{
			get { return "Second Chance Popup"; }
		}
		#endregion
	}
}
