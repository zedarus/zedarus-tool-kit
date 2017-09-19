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
		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Zedarus.ToolKit.Extentions.OneTapGames.SecondChancePopup.SecondChancePopup"/> class.
		/// </summary>
		/// <param name="gameData">Reference to game data in your correct AppController</param>
		/// <param name="apiManager">Reference to API manager.</param>
		/// <param name="wallet">Reference to Wallet object in player's data with required currency.</param>
		/// <param name="localisation">Localisation manager reference.</param>
		/// <param name="videoAdID">Reward video ad placement ID.</param>
		/// <param name="genericPopupID">Generic popup ID.</param>
		/// <param name="popupHeaderStringID">Popup header string localisation ID. Pass null if not required.</param>
		/// <param name="popupMessageStringID">Popup message string localisation ID. Pass null if not required.</param>
		/// <param name="freeChanceButtonLabelID">Localisation ID for "Use for free" button.</param>
		/// <param name="paidChanceButtonLabelID">Localisation ID for "Use for in-game currency" button. String needs to contain placeholder for price number in currency.</param>
		/// <param name="cancelButtonLabelID">Localisation ID for "Cancel" button.</param>
		/// <param name="freeChanceButtonColorID">Color ID for "Use for free" button.</param>
		/// <param name="paidChanceButtonColorID">Color ID for "Use for in-game currency" button.</param>
		/// <param name="cancelButtonColorID">Color ID for "Cancel" button.</param>
		public SecondChancePopup(GameData gameData, APIManager apiManager, Wallet wallet, LocalisationManager localisation, string videoAdID, string genericPopupID, 
			object popupHeaderStringID, object popupMessageStringID, 
			object freeChanceButtonLabelID, object paidChanceButtonLabelID, object cancelButtonLabelID, 
			int freeChanceButtonColorID = 0, int paidChanceButtonColorID = 0, int cancelButtonColorID = 0) : base(apiManager, localisation, genericPopupID, popupHeaderStringID, popupMessageStringID)
		{
			_data = gameData.Get<SecondChancePopupData>().First;

			if (_data == null)
			{
				throw new UnityException("No SecondChancePopupData found in game data");
			}

			_wallet = wallet;

			_videoAdID = videoAdID;
			_sessions = 0;

			apiManager.Ads.CacheRewardVideos(_videoAdID, true);

			CreateButtonKeys(BUTTON_FREE, freeChanceButtonLabelID, freeChanceButtonColorID);
			CreateButtonKeys(BUTTON_PAID, paidChanceButtonLabelID, paidChanceButtonColorID);
			CreateButtonKeys(BUTTON_CANCEL, cancelButtonLabelID, cancelButtonColorID);
		}
		#endregion

		#region Controls
		internal override void RegisterSessionStart()
		{
			_newSession = true;
		}

		internal override void RegisterSessionEnd()
		{
			_sessions++;
		}

		public bool DisplayPopup(UIManager uiManager, int score, System.Action<bool> callback, GameObject adBlock)
		{
			if (CanUseSecondChance(score))
			{
				AssignAdBlock(adBlock);
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

			if (!_data.Enabled)
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
			ActivateAdBlock();
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
			Zedarus.ToolKit.DelayedCall.Create(WaitAndCheckForReward, 2f);
		}

		private void WaitAndCheckForReward()
		{
			DeactivateAdBlock();
			if (_newSession)
			{
				LogAnalytics("reward - failure");
				Decline();
			}
		}

		private void OnSecondChanceRewardVideoReward(int productID)
		{
			DeactivateAdBlock();
			if (_newSession)
			{
				LogAnalytics("reward - success");
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
