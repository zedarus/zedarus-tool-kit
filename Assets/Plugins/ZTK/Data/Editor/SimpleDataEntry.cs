using System;

namespace Zedarus.Toolkit.Data
{
	public class SimpleDataEntry : IDataEntry
	{
		private string _guid;
		private string _name;
		private int _value;

		public SimpleDataEntry(string guid, string name, int value)
		{
			_guid = guid;
			_name = name;
			_value = value;
		}

		public string GUID { get { return _guid; }}
		public string EditorName { get { return _name; } }
		public Type EntryType { get { return typeof(int); } }

		public object GetEditorValue(Type type)
		{
			return _value;
		}
	}
}