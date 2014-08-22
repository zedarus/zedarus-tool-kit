using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Gameplay.Views
{
	public class ModelView : BaseView 
	{
		private int _modelID;
		
		public int ModelID
		{
			get { return _modelID; }
			set { _modelID = value; }
		}
		
		public void Show()
		{
			ShowActions();
		}
		
		public void Hide()
		{
			HideActions();
		}
		
		protected virtual void ShowActions()
		{
		}
		
		protected virtual void HideActions()
		{
		}
	}
}
