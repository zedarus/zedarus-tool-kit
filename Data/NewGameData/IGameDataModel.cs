using System;

namespace Zedarus.Toolkit.Data.New.Game
{
	public interface IGameDataModel
	{
		#if UNITY_EDITOR
		void RenderForm();
		string ListName { get; }
		void CopyValuesFrom(IGameDataModel data);
		#endif
	}
}

