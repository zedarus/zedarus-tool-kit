using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Game;

namespace Zedarus.ToolKit.API
{
	public interface IPromoWrapperInterface
	{
		#region Events
		event Action<IDictionary> ProcessUserDataFromLocalNotification;
		event Action<string, string> ProcessRemoteUnlockFeature;
		event Action<string, int> ProcessRemoteUnlockResource;
		event Action<Dictionary<string, string>> ProcessRemoteUnlockParams;
		#endregion

		#region Controls
		void RequestNotificationsPermission();
		void ClearLocalNotifications();
		void ScheduleLocalNotification(string text, string action, DateTime date, PromoLocalNotifications.RepeatInterval repeat, IDictionary userInfo);
		void RedeemCode(string code);
		void RestoreRewards();
		#endregion

		#region Queries
		#endregion
	}
}
