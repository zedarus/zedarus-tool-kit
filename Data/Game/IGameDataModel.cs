using System;

namespace Zedarus.ToolKit.Data.Game
{
	public interface IGameDataModel
	{
		int ID { get; }
		void OverrideValuesFrom(string json);
		#if UNITY_EDITOR
		void SetDataReference(GameData dataReference);
		void RenderForm(bool included);
		string ListName { get; }
		void CopyValuesFrom(IGameDataModel data, bool copyID);
		#endif
	}
}

