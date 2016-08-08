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
			AnalyticsResult result = Analytics.CustomEvent(eventName);
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
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() {}
		protected override void RemoveEventListeners() {}
		#endregion
	}
}
