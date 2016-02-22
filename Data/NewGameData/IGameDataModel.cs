using System;

namespace Zedarus.Toolkit.Data.New.Game
{
	public interface IGameDataModel
	{
		#if UNITY_EDITOR
		bool RenderForm(string actionButtonLabel);
		string ListName { get; }
		void CopyValuesFrom(IGameDataModel data);
		#endif
	}
}

