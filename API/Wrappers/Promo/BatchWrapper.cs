using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Data.Game;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
#if API_PROMO_BATCH
using Batch;
#endif

namespace Zedarus.ToolKit.API
{
	public class BatchWrapperSettings : APIWrapperSettings
	{
		private string _apiKey = "";

		// TODO: this is madness. We need completely change the way parameters are passed into
		// API controllers and use dictioneries instead with int or enum keys
		public BatchWrapperSettings(object[] settings) : base(settings)
		{
			Assert.IsTrue(settings.Length > 0, "Incorrect number of parameters for Batch wrapper");
			Assert.IsTrue(settings[0].GetType() == typeof(string), "First parameter must be string");

			_apiKey = settings[0].ToString();
		}

		public string APIKey
		{
			get { return _apiKey; }
		}
	}

	public class BatchWrapper : APIWrapper<BatchWrapper>, IPromoWrapperInterface 
	{
		#region Parameters
		#endregion

		#region Events
		public event Action<IDictionary> ProcessUserDataFromLocalNotification;
		#endregion

		#region Properties
		#if API_PROMO_BATCH
		private BatchPlugin _plugin = null;
		#endif
		#endregion

		#region Setup
		protected override void Setup(APIWrapperSettings settings) 
		{
			#if API_PROMO_BATCH
			BatchWrapperSettings batchSettings = settings as BatchWrapperSettings;
			if (batchSettings != null)
			{
				GameObject batchGO = new GameObject("Batch");
				GameObject.DontDestroyOnLoad(batchGO);

				_plugin = batchGO.AddComponent<BatchPlugin>();

				Config config = new Config();
				#if UNITY_IPHONE
				config.IOSAPIKey = batchSettings.APIKey;
				#elif UNITY_ANDROID
				config.AndroidAPIKey = batchSettings.APIKey;
				#endif

				_plugin.StartPlugin(config);
			}
			#endif

			SendInitializedEvent();	// TODO: temp
		}

		protected override APIWrapperSettings ParseSettings(object[] settings)
		{
			return new BatchWrapperSettings(settings);
		}
		#endregion

		#region Controls
		public void RequestNotificationsPermission()
		{
			#if API_PROMO_BATCH
			// TODO: check if this will also work for local notifications
			_plugin.Push.RegisterForRemoteNotifications();

			#if UNITY_IOS
			UnityEngine.iOS.NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
			#endif
			#endif
		}

		public void ClearLocalNotifications()
		{
			#if UNITY_IOS
			int attempts = 64;
			while (UnityEngine.iOS.NotificationServices.localNotificationCount > 0 && attempts > 0) 
			{
				foreach (UnityEngine.iOS.LocalNotification notif in UnityEngine.iOS.NotificationServices.localNotifications)
				{
					if (notif.userInfo != null)
					{
						if (ProcessUserDataFromLocalNotification != null)
						{
							ProcessUserDataFromLocalNotification(notif.userInfo);
						}
					}
				}

				UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
				attempts--;
			}

			UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
			#endif
		}

		public void ScheduleLocalNotification(string text, string action, DateTime date, PromoLocalNotifications.RepeatInterval repeat, IDictionary userInfo)
		{
			#if UNITY_IOS
			var notif = new UnityEngine.iOS.LocalNotification();

			notif.alertBody = text;
			notif.fireDate = date;

			if (!string.IsNullOrEmpty(action.Trim()))
			{
				notif.alertAction = action;
			}

			switch (repeat)
			{
				case PromoLocalNotifications.RepeatInterval.Minute:
					notif.repeatInterval = UnityEngine.iOS.CalendarUnit.Minute;
					break;
				case PromoLocalNotifications.RepeatInterval.Hourly:
					notif.repeatInterval = UnityEngine.iOS.CalendarUnit.Hour;
					break;
				case PromoLocalNotifications.RepeatInterval.Daily:
					notif.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
					break;
				case PromoLocalNotifications.RepeatInterval.Weekly:
					notif.repeatInterval = UnityEngine.iOS.CalendarUnit.Week;
					break;
				case PromoLocalNotifications.RepeatInterval.Monthly:
					notif.repeatInterval = UnityEngine.iOS.CalendarUnit.Month;
					break;
			}

			if (userInfo != null)
			{
				notif.userInfo = userInfo;
			}

			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notif);
			#endif
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() {}
		protected override void RemoveEventListeners() {}
		#endregion

		#region Event Handlers
		#endregion
	}
}
