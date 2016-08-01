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

