using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

namespace Zedarus.ToolKit.API
{
	public class UnityAnalyticsWrapper : APIWrapper<UnityAnalyticsWrapper>, IAnalyticsWrapperInterface 
	{	
		#region Setup
		protected override void Setup(APIWrapperSettings settings) 
		{
			
		}

		protected override APIWrapperSettings ParseSettings(object[] settings)
		{
			return null;
		}
		#endregion

		#region Controls
		public void LogEvent(string eventName)
		{
			#if API_ANALYTICS_UNITY
			AnalyticsResult result = Analytics.CustomEvent(eventName, null);
			#endif

			/*if (GlobalSettings.Instance.DevelopmentBuild)
			{
				switch (result)
				{
					case AnalyticsResult.TooManyRequests:
						Zedarus.ToolKit.ZedLogger.Log("Too many events generated");
						break;
					case AnalyticsResult.TooManyItems:
						Zedarus.ToolKit.ZedLogger.Log("Too many parameters for event");
						break;
					case AnalyticsResult.SizeLimitReached:
						Zedarus.ToolKit.ZedLogger.Log("Parameters for event are too long");
						break;
				}
				TraceEvent(eventName);
			}*/
		}

		public void LogEvent(string eventName, Dictionary<string,object> parameters)
		{
			#if API_ANALYTICS_UNITY
			AnalyticsResult result = Analytics.CustomEvent(eventName, parameters);
			#endif

			/*if (GlobalSettings.Instance.DevelopmentBuild)
			{
				switch (result)
				{
					case AnalyticsResult.TooManyRequests:
						Zedarus.ToolKit.ZedLogger.Log("Too many events generated");
						break;
					case AnalyticsResult.TooManyItems:
						Zedarus.ToolKit.ZedLogger.Log("Too many parameters for event");
						break;
					case AnalyticsResult.SizeLimitReached:
						Zedarus.ToolKit.ZedLogger.Log("Parameters for event are too long");
						break;
				}
				TraceEvent(eventName, parameters);
			}*/
		}

		public void LogPurchase(string product, decimal price, string currency)
		{
			//Analytics.Transaction(product, price, currency, null, null);
			//TraceEvent("Log Purcahse", "product", product, "price", price.ToString(), "currency", currency);
		}
		#endregion

		#region Helpers
		private void TraceEvent(string eventName, params string[] parameters)
		{
			#if DEV_BUILD
			string trace = "Report UnityAnalytics event: \"" + eventName + "\"";
			if (parameters != null && parameters.Length % 2 == 0)
			{
				trace += "\nParameters:\n";
				for (int i = 0; i < parameters.Length; i += 2)
				{
					trace += "\t" + parameters[i] + ": " + parameters[i+1] + "\n";
				}
			}
			if (GlobalSettings.Instance.DevelopmentBuild) Debug.Log(trace);
			#endif
		}

		private void TraceEvent(string eventName, Dictionary<string, object> parameters)
		{
			#if DEV_BUILD
			string trace = "Report Flurry event: \"" + eventName + "\"";
			if (parameters != null)
			{
				trace += "\nParameters:\n";
				foreach(KeyValuePair<string, object> item in parameters)
				{
					trace += "\t" + item.Key + ": " + item.Value.ToString() + "\n";
				}
			}
			if (GlobalSettings.Instance.DevelopmentBuild) Debug.Log(trace);
			#endif
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() {}
		protected override void RemoveEventListeners() {}
		#endregion
	}
}
