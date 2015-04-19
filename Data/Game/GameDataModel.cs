using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using LitJson;
using SimpleSQL;
using Zedarus.ToolKit.Helpers;

namespace Zedarus.ToolKit.Data.Game.Models
{
	public class GameDataModel
	{
		protected int _id;
		protected bool _enabled;
		protected bool _deleted;
		protected List<string> _indexes;

		public GameDataModel() 
		{
		}

		public GameDataModel(int id, bool enabled, bool deleted)
		{
			_id = id;
			_enabled = enabled;
			_deleted = deleted;
		}

		public GameDataModel(JsonData json)
		{
			if (json != null)
			{
				_id = GetInt(json, "id");
				_enabled = GetBool(json, "enabled");
				_deleted = GetBool(json, "deleted");
			}
		}

		public GameDataModel(List<SimpleDataColumn> columns, SimpleDataRow row)
		{
			if (columns != null && row != null)
			{
				_id = GetInt(columns, row, "id");
				_enabled = GetBool(columns, row, "enabled");
				_deleted = GetBool(columns, row, "deleted");
			}
		}

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

		public bool Deleted
		{
			get { return _deleted; }
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

		#region SQLite
		protected object GetObject(List<SimpleDataColumn> columns, SimpleDataRow row, string field)
		{
			int index = 0;
			for (int i = 0; i < columns.Count; i++)
			{
				if (columns[i].name.Equals(field))
				{
					index = i;
					break;
				}
			}
			return row[index];
		}

		protected string GetString(List<SimpleDataColumn> columns, SimpleDataRow row, string field)
		{
			object o = GetObject(columns, row, field);
			if (o != null)
				return o.ToString();
			else
				return null;
		}

		protected int GetInt(List<SimpleDataColumn> columns, SimpleDataRow row, string field)
		{
			int result;
			if (int.TryParse(GetString(columns, row, field), out result))
				return result;
			else
				return 0;
		}
		
		protected float GetFloat(List<SimpleDataColumn> columns, SimpleDataRow row, string field)
		{
			float result;
			if (float.TryParse(GetString(columns, row, field), out result))
				return result;
			else
				return 0f;
		}
		
		protected bool GetBool(List<SimpleDataColumn> columns, SimpleDataRow row, string field)
		{
			return GetInt(columns, row, field) > 0;
		}

		public string GetInsertQuery(string table, string[] additionalFields = null, string[] additionalValues = null)
		{
			string[] fields = GetDBFields();
			string[] values = GetDBValues();
			if (additionalFields != null) fields = AddToArray(fields, additionalFields);
			if (additionalValues != null) values = AddToArray(values, additionalValues);
			return "INSERT INTO " + table + " (" + string.Join(",", fields) + ") VALUES (" + string.Join(",", EscapeDVValues(values)) + ")";
		}

		public string GetUpdateQuery(string table, string[] additionalFields = null, string[] additionalValues = null, string additionalSearchQuery = null)
		{
			string[] fields = GetDBFields();
			string[] values = GetDBValues();
			if (additionalFields != null) fields = AddToArray(fields, additionalFields);
			if (additionalValues != null) values = AddToArray(values, additionalValues);
			values = EscapeDVValues(values);
			string[] updates = new string[fields.Length];
			for (int i = 0; i < updates.Length; i++)
			{
				updates[i] = fields[i] + " = " + values[i];
			}
			return "UPDATE " + table + " SET " + string.Join(",", updates) +  " WHERE id = " + _id.ToString() + (additionalSearchQuery != null ? (" AND " + additionalSearchQuery) : "");
		}

		protected virtual string[] GetDBFields()
		{
			return new string[] {"id", "enabled", "deleted"};
		}

		protected virtual string[] GetDBValues()
		{
			return new string[] {_id.ToString(), (_enabled ? 1 : 0).ToString(), (_deleted ? 1 : 0).ToString()};
		}

		public virtual string GetDBTable()
		{
			return "table";
		}

		protected string[] EscapeDVValues(string[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				string v = values[i];
				values[i] = "\"";
				if (v != null) values[i] += v.Replace("\"", "\"\"");
				values[i] += "\"";
			}
			return values;
		}
		
		protected string[] AddToArray(string[] oldFields, params string[] newFields)
		{
			string[] fields = new string[oldFields.Length + newFields.Length];
			oldFields.CopyTo(fields, 0);
			newFields.CopyTo(fields, oldFields.Length);
			return fields;
		}

		protected string[] AddArrayToArray(string[] oldFields, string[] newFields)
		{
			string[] fields = new string[oldFields.Length + newFields.Length];
			oldFields.CopyTo(fields, 0);
			newFields.CopyTo(fields, oldFields.Length);
			return fields;
		}
		#endregion
	}
}
