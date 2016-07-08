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
		public void RequestNotificationsPermission()
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
			}
		}

		protected override void RemoveEventListeners() 
		{
			base.RemoveEventListeners();

			foreach (IPromoWrapperInterface wrapper in Wrappers)
			{
				wrapper.ProcessUserDataFromLocalNotification -= OnProcessUserDataFromLocalNotification;
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
		#endregion

		#region Getters
		protected IPromoWrapperInterface Wrapper
		{
			get { return (IPromoWrapperInterface)CurrentWrapperBase; }
		}
		#endregion
	}
}
