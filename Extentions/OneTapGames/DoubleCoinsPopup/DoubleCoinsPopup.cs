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
	public class DoubleCoinsPopup : Extention
	{
		#region Properties
		private DoubleCoinsPopupData _data;
		private bool _newSession = false;
		private bool _openSession = false;
		private string _videoAdID = null;
		private int _sessions = 0;
		private int _coinsEarned = 0;
		#endregion

		#region Events
		public event System.Action<int> DoubleCoinsConfirm;
		public event System.Action DoubleCoinsCancel;
		#endregion

		#region Init
		public DoubleCoinsPopup(GameData gameData, APIManager apiManager, string videoAdID) : base(apiManager)
		{
			_data = gameData.Get<DoubleCoinsPopupData>().First;
			_videoAdID = videoAdID;
			_sessions = 0;
		}
		#endregion

		#region Controls
		public void RegisterSessionStart()
		{
			_newSession = true;
			_openSession = true;
		}

		public void RegisterSessionEnd()
		{
			if (_openSession)
			{
				_openSession = false;
				_sessions++;
			}
		}

		public bool DisplayPopup(UIManager uiManager, string genericPopupID, string popupHeader, string popupMessage, int coinsEarned, 
			string doubleButtonLabel, string cancelButtonLabel, 
			int doubleButtonLabelColorID = 0, int cancelButtonColorID = 0)
		{

			if (CanUse(coinsEarned))
			{
				_coinsEarned = coinsEarned;

				List<Zedarus.ToolKit.UI.UIGenericPopupButtonData> buttons = new List<Zedarus.ToolKit.UI.UIGenericPopupButtonData>();

				buttons.Add(new Zedarus.ToolKit.UI.UIGenericPopupButtonData(
					doubleButtonLabel, OnDoubleConfirmed, doubleButtonLabelColorID
				));

				buttons.Add(new Zedarus.ToolKit.UI.UIGenericPopupButtonData(
					cancelButtonLabel, OnCancel, cancelButtonColorID
				));

				uiManager.OpenPopup(genericPopupID, new Zedarus.ToolKit.UI.UIGenericPopupData(
					popupHeader, popupMessage, buttons.ToArray()
				));

				return true;
			}
			else
			{
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
			if (DoubleCoinsConfirm != null)
			{
				DoubleCoinsConfirm(_coinsEarned);
			}
		}

		private void Decline()
		{
			_newSession = false;
			if (DoubleCoinsCancel != null)
			{
				DoubleCoinsCancel();
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
