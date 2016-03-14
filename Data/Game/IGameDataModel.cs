using System;

namespace Zedarus.ToolKit.Data.Game
{
	public interface IGameDataModel
	{
		#if UNITY_EDITOR
		int ID { get; }
		void RenderForm(bool included);
		string ListName { get; }
		void CopyValuesFrom(IGameDataModel data, bool copyID);
		#endif
		void OverrideValuesFrom(string json);
	}
}

