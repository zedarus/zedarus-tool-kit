using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.Traffico.Settings;

namespace Zedarus.ToolKit.API
{
	#if API_FLURRY_P31
	public class FlurryWrapper : APIWrapper<FlurryWrapper>, IAnalyticsWrapperInterface 
	{	
		#region Setup
		protected override void Setup() 
		{
			#if UNITY_IPHONE
			FlurryAnalytics.startSession(APIManager.Instance.Settings.FlurryKey);
			FlurryAnalytics.setSessionReportsOnCloseEnabled(true);
			FlurryAnalytics.setSessionReportsOnPauseEnabled(true);
			#elif UNITY_ANDROID
			FlurryAndroid.onStartSession(APIManager.Instance.Settings.FlurryKey, false, false);
			FlurryAndroid.setLogEnabled(GlobalSettings.Instance.DevelopmentBuild);
			#endif
		}
		#endregion
		
		#region Controls
		public void LogEvent(string eventName)
		{
			#if UNITY_IPHONE
			FlurryAnalytics.logEvent(eventName, false);
			#elif UNITY_ANDROID
			FlurryAndroid.logEvent(eventName, false);
			#endif
			if (GlobalSettings.Instance.DevelopmentBuild) TraceEvent(eventName, null);
		}
		
		public void LogEvent(string eventName, Dictionary<string,string> parameters)
		{
			#if UNITY_IPHONE
			FlurryAnalytics.logEventWithParameters(eventName, parameters, false);
			#elif UNITY_ANDROID
			FlurryAndroid.logEvent(eventName, parameters, false);
			#endif
			if (GlobalSettings.Instance.DevelopmentBuild) TraceEvent(eventName, parameters);
		}
		#endregion
		
		private void TraceEvent(string eventName, Dictionary<string, string> parameters)
		{
			string trace = "Report Flurry event: \"" + eventName + "\"";
			if (parameters != null)
			{
				trace += "\nParameters:\n";
				foreach(KeyValuePair<string, string> item in parameters)
				{
					trace += "\t" + item.Key + ": " + item.Value + "\n";
				}
			}
			if (GlobalSettings.Instance.DevelopmentBuild) Debug.Log(trace);
		}
		
		#region Event Listeners
		protected override void CreateEventListeners() {}
		protected override void RemoveEventListeners() {}
		#endregion
	}
	#endif
}
