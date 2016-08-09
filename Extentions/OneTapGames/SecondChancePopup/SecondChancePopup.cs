using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.UI;
using Zedarus.ToolKit.API;

namespace Zedarus.ToolKit.Extentions.OneTapGames.SecondChancePopup
{
	public class SecondChancePopup : Extention
	{
		#region Properties
		private APIManager _api;
		private SecondChancePopupData _data;
		private bool _newSession = false;
		private string _videoAdID = null;
		private int _sessions = 0;
		#endregion

		#region Events
		public event System.Action<int> PayForSecondChance;
		public event System.Action UseSecondChance;
		public event System.Action DeclineSecondChance;
		#endregion

		#region Init
		public SecondChancePopup(GameData gameData, APIManager apiManager, string videoAdID) : base()
		{
			_data = gameData.Get<SecondChancePopupData>().First;
			_api = apiManager;
			_videoAdID = videoAdID;
			_sessions = 0;
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

		public bool DisplayPopup(UIManager uiManager, string genericPopupID, string popupMessage, int score, int coinsBalance, 
			string freeChanceButtonLabel, string paidChanceButtonLabel, string cancelButtonLabel)
		{

			if (CanUseSecondChance(score))
			{
				List<Zedarus.ToolKit.UI.UIGenericPopupButtonData> buttons = new List<Zedarus.ToolKit.UI.UIGenericPopupButtonData>();

				buttons.Add(new Zedarus.ToolKit.UI.UIGenericPopupButtonData(
						freeChanceButtonLabel, OnFreeSecondChanceConfirmed
					));

				int price = _data.Price;
				if (coinsBalance >= price)
				{
					string label = string.Format(paidChanceButtonLabel, price);
					buttons.Add(new Zedarus.ToolKit.UI.UIGenericPopupButtonData(label, OnPaidSecondChanceConfirmed));
				}

				buttons.Add(new Zedarus.ToolKit.UI.UIGenericPopupButtonData(
						cancelButtonLabel, OnSecondChanceDenied
					));

				uiManager.OpenPopup(genericPopupID, new Zedarus.ToolKit.UI.UIGenericPopupData(
						popupMessage, null, buttons.ToArray()
					));

				return true;
			}
			else
			{
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
			if (UseSecondChance != null)
			{
				UseSecondChance();
			}
		}

		private void Decline()
		{
			_newSession = false;
			if (DeclineSecondChance != null)
			{
				DeclineSecondChance();
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
//			Analytics.Instance.LogSecondChanceUse(Analytics.SecondChanceAction.UseAds);

			_api.Ads.ShowRewardedVideo(_videoAdID, OnSecondChanceRewardVideoClose, OnSecondChanceRewardVideoReward, 0);
		}

		private void OnPaidSecondChanceConfirmed()
		{
//			Analytics.Instance.LogSecondChanceUse(Analytics.SecondChanceAction.UseCoins);

			if (PayForSecondChance != null)
			{
				PayForSecondChance(_data.Price);
			}

			Use();
		}

		private void OnSecondChanceDenied()
		{
//			Analytics.Instance.LogSecondChanceUse(Analytics.SecondChanceAction.Decline);

			Decline();

//			_secondChanceDeclined = true;
//			GameOverEnter();
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
//			if (_state.CurrentState == State.GameOver)
//			{
//				_secondChanceDeclined = true;
//				GameOverEnter();
//			}
		}

		private void OnSecondChanceRewardVideoReward(int productID)
		{
			if (_newSession)
			{
				Use();
			}
//			if (_state.CurrentState == State.GameOver)
//			{
//				_fist.Ressurect();
//				_state.ChangeState(State.Playing);
//			}
		}
		#endregion
	}
}
