using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.Events
{
	internal class EventRegister
	{
		#region Properties
		private List<int> _events;
		#endregion

		#region Init
		public EventRegister()
		{
			_events = new List<int>();
		}
		#endregion

		#region Controls
		public bool Register(int eventID)
		{
			if (_events.Contains(eventID))
			{
				Debug.LogError("Event with ID " + eventID + " already exists, pick another one!");
				return false;
			} 
			else
			{
				_events.Add(eventID);
				return true;
			}
		}
		#endregion

		#region Queries
		public bool IsRegistered(int eventID)
		{
			return _events.Contains(eventID);
		}
		#endregion
	}
}
