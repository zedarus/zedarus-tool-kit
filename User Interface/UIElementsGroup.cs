using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UserInterface
{
	#if !ZTK_DISABLE_NGUI
	[RequireComponent(typeof(UIElementAnimationsController))]
	public class UIElementsGroup : MonoBehaviour 
	{	
		#region Parameters
		private UIElementAnimationsController _animationsController;
		#endregion

		#region Events
		public event System.Action<UIElementsGroup> Opened;
		public event System.Action<UIElementsGroup> Closed;
		#endregion

		#region Main Methods
		public virtual void Init()
		{
			_animationsController = GetComponent<UIElementAnimationsController>();
			_animationsController.Init();
		}
		
		public virtual void Cycle(float deltaTime) {}
		#endregion

		#region Controls
		public virtual float Show()
		{
			float delay = _animationsController.Show();
			StopCoroutine("SendOpenedEvent");
			StopCoroutine("SendClosedEvent");
			StartCoroutine("SendOpenedEvent", delay);
			return delay;
		}

		public virtual float Hide()
		{
			float delay = _animationsController.Hide();
			StopCoroutine("SendOpenedEvent");
			StopCoroutine("SendClosedEvent");
			StartCoroutine("SendClosedEvent", delay);
			return delay;
		}
		#endregion

		#region Event Senders
		private IEnumerator SendOpenedEvent(float delay)
		{
			yield return new WaitForSeconds(delay);
			if (Opened != null)
				Opened(this);
		}

		private IEnumerator SendClosedEvent(float delay)
		{
			yield return new WaitForSeconds(delay);
			if (Closed != null)
				Closed(this);
		}
		#endregion
	}
	#endif
}
