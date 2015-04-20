using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UserInterface.NGUI
{
	#if ZTK_NGUI
	public class UIElementAnimation : MonoBehaviour, IUIElementAnimation
	{
		#region Parameters
		[SerializeField, HideInInspector]
		protected float _showDuration = 1f;
		[SerializeField, HideInInspector]
		protected float _hideDuration = 1f;
		#endregion

		public virtual void Init()
		{
			
		}

		#region Animations
		public virtual void Show()
		{

		}
		
		public virtual void Hide()
		{

		}
		
		public virtual void Reset()
		{

		}
		
		public virtual float ShowDuration
		{
			get { return _showDuration; }
			set { _showDuration = value; }
		}
		
		public virtual float HideDuration
		{
			get { return _hideDuration; }
			set { _hideDuration = value; }
		}
		
		public virtual GameObject ObjectReference 
		{ 
			get { return gameObject; }
		}
		#endregion
	}
	#endif
}
