using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.UserInterface
{
	public class UIController : MonoBehaviour
	{
		#region Parameters
		[SerializeField]
		private UICamera _camera;
		[SerializeField]
		public UIElementsGroup _fadeInGroup;
		private List<UIElementsGroup> _groups = new List<UIElementsGroup>();
		#endregion

		#region Unity Methods
		private void Start()
		{
			Init();
		}

		private void Update()
		{
			Cycle(Time.deltaTime);
		}
		#endregion

		#region Controls
		public virtual float Show()
		{
			float maxDuration = 0;
			float duration = 0;
			foreach (UIElementsGroup group in _groups)
			{
				duration = group.Show();
				if (duration > maxDuration) maxDuration = duration;
			}
			return maxDuration;
		}

		public virtual float Hide()
		{
			BlockAllInput();
			float maxDuration = 0;
			float duration = 0;
			foreach (UIElementsGroup group in _groups)
			{
				duration = group.Hide();
				if (duration > maxDuration) maxDuration = duration;
			}
			return maxDuration;
		}
		#endregion

		#region Main Methods
		protected virtual void Init()
		{
			if (_fadeInGroup != null) AddGroup(_fadeInGroup);
			foreach (UIElementsGroup group in _groups)
				group.Init();
		}

		protected virtual void Cycle(float deltaTime)
		{
			foreach (UIElementsGroup group in _groups)
				group.Cycle(deltaTime);
		}

		protected void AddGroup(UIElementsGroup group)
		{
			_groups.Add(group);
		}
		#endregion

		protected virtual void BlockAllInput()
		{
			_camera.useTouch = _camera.useController = _camera.useMouse = _camera.useKeyboard = false;
		}
	}
}
