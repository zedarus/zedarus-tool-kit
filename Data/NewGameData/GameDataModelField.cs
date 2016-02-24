using UnityEngine;
using System.Collections;

namespace Zedarus.Toolkit.Data.New.Game
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class DataField : System.Attribute
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

		public DataField(string editorLabel)
		{
			_editorLabel = editorLabel;
		}

		public string EditorLabel
		{
			get { return _editorLabel; }
		}
	}

	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class DataValidateClamp : System.Attribute
	{
		private int _min = 0;
		private int _max = 0;
		private float _minFloat = 0f;
		private float _maxFloat = 0f;

		public DataValidateClamp(int min, int max)
		{
			_min = min;
			_max = max;
		}

		public DataValidateClamp(float min, float max)
		{
			_minFloat = min;
			_maxFloat = max;
		}

		public int Min
		{
			get { return _min; }
		}

		public int Max
		{
			get { return _max; }
		}

		public float MinFloat
		{
			get { return _minFloat; }
		}

		public float MaxFloat
		{
			get { return _maxFloat; }
		}
	}
}
