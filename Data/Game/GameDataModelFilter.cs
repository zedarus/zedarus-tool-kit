using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace Zedarus.ToolKit.Data.Game
{
	public class GameDataModelFilter
	{
		#region Properties
		private string _propertyName = null;
		private string _filterValue = null;
		#endregion

		#region Init
		public GameDataModelFilter()
		{
		}
		#endregion

		#region Queries
		public bool CompareToString(object value)
		{
			string s = value.ToString();
			if (s != null)
			{
				return s.Equals(FilterValue);
			}
			return true;
		}

		public bool CompareToInt(object value)
		{
			int v = 0;
			int i = 0;
			if (int.TryParse(value.ToString(), out v) && int.TryParse(FilterValue, out i))
			{
				return i.Equals(v);
			}
			return true;
		}

		public bool CompareToFloat(object value)
		{
			float v = 0;
			float i = 0;
			if (float.TryParse(value.ToString(), out v) && float.TryParse(FilterValue, out i))
			{
				return i.Equals(v);
			}
			return true;
		}

		public bool CompareToBool(object value)
		{
			bool v = false;
			bool i = false;
			if (bool.TryParse(value.ToString(), out v) && bool.TryParse(FilterValue, out i))
			{
				return i.Equals(v);
			}
			return true;
		}
		#endregion

		#region Getters
		public string PropertyName
		{
			get { return _propertyName; }
			set { _propertyName = value; }
		}

		public string FilterValue
		{
			get { return _filterValue; }
			set { _filterValue = value; }
		}
		#endregion
	}
}

