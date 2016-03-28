using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.Data.Game
{
	[Serializable]
	public struct DateData
	{
		#region Properties
		[SerializeField]
		private int _year;
		[SerializeField]
		private int _month;
		[SerializeField]
		private int _day;
		#endregion

		#region Initalization
		public DateData(DateTime date)
		{
			_year = date.Year;
			_month = date.Month;
			_day = date.Month;
		}
		#endregion

		#region Controls
		public void SetYear(int year)
		{
			_year = year;
		}

		public void SetMonth(int month)
		{
			_month = month;
		}

		public void SetDay(int day)
		{
			_day = day;
		}
		#endregion

		#region Getters
		public int Year
		{
			get { return _year; }
		}

		public int Month
		{
			get { return _month; }
		}

		public int Day
		{
			get { return _day; }
		}
		#endregion
	}
}

