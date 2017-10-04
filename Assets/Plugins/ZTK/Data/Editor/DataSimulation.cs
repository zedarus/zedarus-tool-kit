using System;
using UnityEngine;

namespace Zedarus.Toolkit.Data
{
	public class DataSimulation : IData
	{
		private GUIContent[] _options;
		private IDataEntry[] _entries;

		public DataSimulation()
		{
			_options = new[] { new GUIContent("One"), new GUIContent("Two") } ;
			_entries = new[] { new SimpleDataEntry("1", "One", 1) };
		}

		public IDataEntry[] GetEntriesOfType(Type t)
		{
			return _entries;
			
			/*if (t == typeof(int))
			{
				return null;
			}

			return null;*/
		}

		public GUIContent[] GetEntriesNamesOfType(Type t)
		{
			return _options;
		}

		private static DataSimulation _instance;
		public static DataSimulation Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DataSimulation();
				}
				
				return _instance;
			}
		}
	}
}