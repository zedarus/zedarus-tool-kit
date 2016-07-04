using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Collections;

namespace Zedarus.ToolKit.Data.Player
{
	[Serializable]
	public class RateMePopup : IPlayerDataModel
	{
		#region Properties
		[SerializeField]
		private int _eventsCounter;
//		[OptionalField]
//		private DateTime _lastTimeDisplayed;
		[SerializeField]
		private int _displayCounter;
		#endregion

		#region Init
		public RateMePopup() 
		{
			SetDefaults(new StreamingContext());
		}

		[OnDeserializing]
		private void SetDefaults(StreamingContext sc)
		{
			_eventsCounter = 0;
			_displayCounter = 0;
//			_lastTimeDisplayed = new DateTime(1986, 7, 21);
		}
		#endregion

		#region Controls
		public bool DisplayPopup(int eventsDelay, bool displayOnce)
		{
			_eventsCounter++;

			if (_eventsCounter >= eventsDelay)
			{
				if (displayOnce && _displayCounter > 0)
				{
					return false;
				}
				else
				{
					_eventsCounter = 0;
					_displayCounter++;

					return true;
				}


//				TimeSpan difference = DateTime.UtcNow - _lastTimeDisplayed;
//				if (Convert.ToInt32(difference.TotalHours) >= hoursDelay)
//				{
//					return true;
//				}
			}

			return false;
		}
		#endregion

		#region Controls
		public void Reset() { }
		public bool Merge(IPlayerDataModel data) 
		{ 
			RateMePopup other = (RateMePopup)data;
			if (other != null)
			{
//				if (other._lastTimeDisplayed.CompareTo(_lastTimeDisplayed) < 0)
//				{
//					_lastTimeDisplayed = other._lastTimeDisplayed;
//				}

				if (other._eventsCounter > _eventsCounter)
				{
					_eventsCounter = other._eventsCounter;
				}

				if (other._displayCounter > _displayCounter)
				{
					_displayCounter = other._displayCounter;
				}
				return true;
			}
			else
			{
				return false; 
			}
		}
		#endregion
	}
}
