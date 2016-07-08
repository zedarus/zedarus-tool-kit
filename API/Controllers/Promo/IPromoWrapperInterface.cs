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
		#endregion

		#region Controls
		void RequestNotificationsPermission();
		void ClearLocalNotifications();
		void ScheduleLocalNotification(string text, string action, DateTime date, PromoLocalNotifications.RepeatInterval repeat, IDictionary userInfo);
		#endregion

		#region Queries
		#endregion
	}
}
