using UnityEngine;
using System.Collections;

namespace Zedarus.Toolkit.Data.New.Game
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class GameDataModelField : System.Attribute
	{
		public enum CustomFieldType
		{
			Default = 1,
			Prefab = 2,
		}

		private string _editorLabel;
		public bool locked = false;
		public CustomFieldType customFieldType = CustomFieldType.Default;
		public System.Type customFieldTypeLimit = null;
		public bool autoRender = true;

		public GameDataModelField(string editorLabel)
		{
			_editorLabel = editorLabel;
		}

		public string EditorLabel
		{
			get { return _editorLabel; }
		}
	}
}
