using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UserInterface
{
	[RequireComponent(typeof(UIElementAnimationsController))]
	public class UIElementsGroup : MonoBehaviour 
	{	
		#region Parameters
		private UIElementAnimationsController _animationsController;
		#endregion

		#region Main Methods
		public virtual void Init()
		{
			_animationsController = GetComponent<UIElementAnimationsController>();
			_animationsController.Init();
		}
		
		public void Cycle(float deltaTime) {}
		#endregion

		#region Controls
		public float Show()
		{
			return _animationsController.Show();
		}

		public float Hide()
		{
			return _animationsController.Hide();
		}
		#endregion
	}
}
