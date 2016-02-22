using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Zedarus.Toolkit.Data.New.Game
{
	[System.Serializable]
	public class GameDataModelBase : IGameDataModel
	{
		#region Properties
		[SerializeField] private int _id;
		#endregion

		#region Initalization
		public GameDataModelBase() : this(0) { }

		public GameDataModelBase(int id)
		{
			_id = id;
		}
		#endregion

		#region Getters
		public int ID
		{
			get { return _id; }
			#if UNITY_EDITOR
			set 
			{
				if (value < 0)
					value = 0;
				_id = value; 
			}
			#endif
		}
		#endregion

		#if UNITY_EDITOR
		public virtual void RenderForm()
		{
			GUI.enabled = false;
			ID = EditorGUILayout.IntField("ID", ID);
			GUI.enabled = true;
		}

		public virtual string ListName { get { return "#" + ID.ToString(); } }

		public virtual void CopyValuesFrom(IGameDataModel data)
		{
			GameDataModelBase other = data as GameDataModelBase;
			if (other != null)
			{
				ID = other.ID;
			}
		}
		#endif
	}
}

