using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.UI;
using Zedarus.ToolKit.API;
using Zedarus.ToolKit.Localisation;

namespace Zedarus.ToolKit.Extentions
{
	public class Extention
	{
		#region Properties
		private APIManager _api;
		#endregion

		#region Init
		public Extention(APIManager api)
		{
			_api = api;
		}
		#endregion

		#region Getters
		protected APIManager API
		{
			get { return _api; }
		}
		#endregion

		#region Analytics
		protected void LogAnalytics(string action)
		{
			API.Analytics.LogEvent(EventName, new Dictionary<string, object> {
				{ "action", action }
			});
		}

		protected virtual string EventName
		{
			get { return "Extention Event Name"; }
		}
		#endregion

		#region Sessions
		internal virtual void RegisterSessionStart()
		{
		}

		internal virtual void RegisterSessionEnd()
		{
		}
		#endregion
	}
}

