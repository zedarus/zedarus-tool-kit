using System;

namespace Zedarus.Toolkit.Data
{
	public interface IDataEntry
	{
		string GUID { get; }
		string EditorName { get; }
		Type EntryType { get; }
		object GetEditorValue(Type type);
	}
}