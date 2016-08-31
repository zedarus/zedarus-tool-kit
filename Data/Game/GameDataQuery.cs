using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;


namespace Zedarus.ToolKit.Data.Game
{
	public class GameDataQuery<T> where T : class, IGameDataModel
	{
		#region Properties
		private T[] _results;
		private FieldInfo[] _fields;
		#endregion

		#region Init
		public GameDataQuery(T[] objects)
		{
			_results = objects;

			FieldInfo[] fields = GameDataModel.GetFields(typeof(T));
			object[] attrs = null;
			List<FieldInfo> selectedFields = new List<FieldInfo>();

			foreach (FieldInfo field in fields)
			{
				attrs = field.GetCustomAttributes(typeof(DataField), true);
				foreach (object attr in attrs)
				{
					DataField fieldAttr = attr as DataField;
					if (fieldAttr != null)
					{
						selectedFields.Add(field);
					}
				}
			}

			_fields = selectedFields.ToArray();
			selectedFields.Clear();
			selectedFields = null;
		}

		public void Clear()
		{
			_results = null;
		}
		#endregion

		#region Filtering
		public GameDataQuery<T> Where(string fieldName, object value, params object[] otherValues)
		{
			object fieldValue = null;
			foreach (FieldInfo field in _fields)
			{
				// TODO: trim fielname here
				if (field.Name.Equals(fieldName))
				{
					List<T> filteredResults = new List<T>();

					foreach (T result in _results)
					{
						fieldValue = field.GetValue(result);
						if (fieldValue.Equals(value))
						{
							filteredResults.Add(result);
						}

						if (otherValues != null && otherValues.Length > 0)
						{
							foreach (object v in otherValues)
							{
								if (fieldValue.Equals(v))
								{
									filteredResults.Add(result);
								}
							}
						}
					}

					_results = filteredResults.ToArray();
					filteredResults.Clear();
					break;
				}
			}

			return this;
		}
		#endregion

		#region Getters
		public T First
		{
			get 
			{ 
				if (_results != null && _results.Length > 0)
				{
					return _results[0];
				}
				else
				{
					return null; 
				}
			}
		}

		public T Random
		{
			get
			{
				if (_results != null && _results.Length > 0)
				{
					return _results[UnityEngine.Random.Range(0, _results.Length)];
				}
				else
				{
					return null; 
				}
			}
		}

		public T[] All
		{
			get { return _results; }
		}

		public int Count
		{
			get { return _results.Length; }
		}
		#endregion
	}
}

