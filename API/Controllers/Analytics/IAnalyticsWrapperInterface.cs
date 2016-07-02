using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public interface IAnalyticsWrapperInterface
	{
		#region Controls
		void LogEvent(string eventName);
		void LogEvent(string eventName, Dictionary<string,object> parameters);
		#endregion
	}
}
