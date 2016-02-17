using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using LitJson;
using Zedarus.ToolKit.Helpers;

namespace Zedarus.ToolKit.Data.Game
{
	public class GameDataModel
	{
		protected int _id;
		protected bool _enabled;
		protected List<string> _indexes;

		public GameDataModel() 
		{
		}

		public GameDataModel(int id, bool enabled)
		{
			_id = id;
			_enabled = enabled;
		}

		public GameDataModel(JsonData json)
		{
			if (json != null)
			{
				_id = GetInt(json, "id");
				_enabled = GetBool(json, "enabled");
			}
		}

		//public GameDataModel(List<SimpleDataColumn> columns, SimpleDataRow row)
		//{
		//	if (columns != null && row != null)
		//	{
		//		_id = GetInt(columns, row, "id");
		//		_enabled = GetBool(columns, row, "enabled");
		//	}
		//}

		public virtual string[] GetIndexes()
		{
			return new string[] {};
		}

		#region Getters
		public int ID
		{
			get { return _id; }
		}

		public bool Enabled
		{
			get { return _enabled; }
		}

		public string[] Indexes
		{
			get { return _indexes.ToArray(); }
		}
		#endregion

		#region JSON
		protected string GetString(JsonData json, string field)
		{
			if (json[field] != null)
				return json[field].ToString();
			else
				return null;
		}

		protected int GetInt(JsonData json, string field)
		{
			int result;
			if (int.TryParse(GetString(json, field), out result))
				return result;
			else
				return 0;
		}

		protected float GetFloat(JsonData json, string field)
		{
			float result;
			if (float.TryParse(GetString(json, field), out result))
				return result;
			else
				return 0f;
		}

		protected bool GetBool(JsonData json, string field)
		{
			return GetInt(json, field) > 0;
		}
		
		protected DateTime ParseTime(JsonData json, string field)
		{
			return GeneralHelper.ParseTime(GetString(json, field));
		}
		#endregion
	}
}
