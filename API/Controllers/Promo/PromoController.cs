using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Settings;
using Zedarus.ToolKit.Events;

namespace Zedarus.ToolKit.API
{
	public class PromoController : APIController 
	{	
		#region Events
		public event Action<int> ProcessRewardFromLocalNotification;
		public event Action<string, string> ProcessRemoteUnlockFeature;
		public event Action<string, int> ProcessRemoteUnlockResource;
		public event Action<Dictionary<string, string>> ProcessRemoteUnlockParams;
		#endregion

		#region Properties
		#endregion

		#region Initialization
		protected override void Setup() { }
		#endregion

		#region Wrappers Initialization
		protected override IAPIWrapperInterface GetWrapperForAPI(int wrapperAPI)
		{
			switch (wrapperAPI)
			{
				case APIs.Promo.Batch:
					return BatchWrapper.Instance;
				default:
					return null;
			}
		}
		#endregion

		#region Controls
		public void RequestNotificationsPermission(float delay = 0f)
		{
			if (delay <= 0)
			{
				RequestNotifications();
			}
			else
			{
				DelayedCall.Create(RequestNotifications, delay, true, true, "Request Push Notifications");
			}
		}

		private void RequestNotifications()
		{
			if (Wrapper != null)
			{
				Wrapper.RequestNotificationsPermission();
			}
		}

		public void ClearLocalNotifications()
		{
			if (Wrapper != null)
			{
				Wrapper.ClearLocalNotifications();
			}
		}

		public void ScheduleLocalNotification(string text, string action, DateTime date, PromoLocalNotifications.RepeatInterval repeat, IDictionary userInfo)
		{
			if (Wrapper != null)
			{
				if (Manager.Settings.LocalNotificationsEnabled)
				{
					Wrapper.ScheduleLocalNotification(text, action, date, repeat, userInfo);
				}
				else
				{
					Debug.Log("Local notification are disabled in API settings");
				}
			}
		}

		public void RedeemCode(string code)
		{
			if (Wrapper != null)	
			{
				Wrapper.RedeemCode(code);
			}
		}

		public void RestoreRewards()
		{
			if (Wrapper != null)	
			{
				Wrapper.RestoreRewards();
			}
		}
		#endregion

		#region Queries

		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			base.CreateEventListeners();

			foreach (IPromoWrapperInterface wrapper in Wrappers)
			{
				wrapper.ProcessUserDataFromLocalNotification += OnProcessUserDataFromLocalNotification;
				wrapper.ProcessRemoteUnlockFeature += OnProcessRemoteUnlockFeature;
				wrapper.ProcessRemoteUnlockResource += OnProcessRemoteUnlockResource;
				wrapper.ProcessRemoteUnlockParams += OnProcessRemoteUnlockParams;
			}
		}

		protected override void RemoveEventListeners() 
		{
			base.RemoveEventListeners();

			foreach (IPromoWrapperInterface wrapper in Wrappers)
			{
				wrapper.ProcessUserDataFromLocalNotification -= OnProcessUserDataFromLocalNotification;
				wrapper.ProcessRemoteUnlockFeature -= OnProcessRemoteUnlockFeature;
				wrapper.ProcessRemoteUnlockResource -= OnProcessRemoteUnlockResource;
				wrapper.ProcessRemoteUnlockParams -= OnProcessRemoteUnlockParams;
			}
		}
		#endregion

		#region Event Handlers
		private void OnProcessUserDataFromLocalNotification(IDictionary userInfo)
		{
			if (ProcessRewardFromLocalNotification != null)
			{
				if (userInfo != null && userInfo.Contains(PromoLocalNotifications.REWARD_ID))
				{
					int rewardID = 0;

					if (int.TryParse(userInfo[PromoLocalNotifications.REWARD_ID].ToString(), out rewardID))
					{
						ProcessRewardFromLocalNotification(rewardID);
					}
				}
			}
		}

		private void OnProcessRemoteUnlockFeature(string feature, string value)
		{
			if (ProcessRemoteUnlockFeature != null)
			{
				ProcessRemoteUnlockFeature(feature, value);
			}
		}

		private void OnProcessRemoteUnlockResource(string resource, int quantity)
		{
			if (ProcessRemoteUnlockResource != null)
			{
				ProcessRemoteUnlockResource(resource, quantity);
			}
		}

		private void OnProcessRemoteUnlockParams(Dictionary<string, string> parameters)
		{
			if (ProcessRemoteUnlockParams != null)
			{
				ProcessRemoteUnlockParams(parameters);
			}
		}
		#endregion

		#region Getters
		protected IPromoWrapperInterface Wrapper
		{
			get { return (IPromoWrapperInterface)CurrentWrapperBase; }
		}
		#endregion
	}
}
