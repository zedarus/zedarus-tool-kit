using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UI.Elements
{
	[System.Serializable]
	public class UICarouselScrollSettings
	{
		[SerializeField]
		private UICarouselScrollLayer[] _layers;

		[Header("Scroll Settings")]
		[SerializeField] 
		[Range(0.05f, 1f)]
		private float _easing = 0.9f;
		[SerializeField] 
		[Range(0.05f, 4f)]
		private float _easingTimeScale = 1f;
		[SerializeField]
		[Range(0f, 1f)]
		private float _swipeThreshold = 0.5f;	// TODO: rename

		#region Getters
		public float GetEasing(float deltaTime)
		{
			return _easing *  Mathf.Clamp(deltaTime * _easingTimeScale, 0f, 1f);
		}

		public float SwipeThreshold
		{
			get { return _swipeThreshold; }
		}

		public UICarouselScrollLayer[] Layers
		{
			get { return _layers; }
		}
		#endregion
	}
}
