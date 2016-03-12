using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public interface IAnalyticsWrapperInterface
	{
		#region Events
		#endregion
		
		#region Controls
		void LogEvent(string eventName);
		void LogEvent(string eventName, Dictionary<string,object> parameters);
		void LogPurchase(string product, decimal price, string currency);
		#endregion
		
		#region Queries
		#endregion
	}
}
