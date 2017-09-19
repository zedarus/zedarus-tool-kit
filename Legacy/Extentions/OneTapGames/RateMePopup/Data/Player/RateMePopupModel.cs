using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Collections;
using Zedarus.ToolKit.UI;
using Zedarus.ToolKit.Data.Player;

namespace Zedarus.ToolKit.Extentions.OneTapGames.RateMePopup
{
	[Serializable]
	public class RateMePopupModel : IPlayerDataModel
	{
		#region Properties
		[OptionalField]
		private int _eventsCounter;
		[OptionalField]
		private int _displayCounter;
		#endregion

		#region Init
		public RateMePopupModel() 
		{
			SetDefaults(new StreamingContext());
		}

		[OnDeserializing]
		private void SetDefaults(StreamingContext sc)
		{
			_eventsCounter = 0;
			_displayCounter = 0;
		}
		#endregion

		#region Controls
		public void IterateEvent()
		{
			_eventsCounter++;
		}

		public bool CanDisplayBetweenLevels(int eventsDelay, bool displayOnce)
		{
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
			}

			return false;
		}
		#endregion

		#region Controls
		public void Reset() { }
		public bool Merge(IPlayerDataModel data) 
		{ 
			RateMePopupModel other = (RateMePopupModel)data;
			if (other != null)
			{
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
