using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UI.Elements
{
	[System.Serializable]
	public class UICarouselScrollSettings
	{
		#region Parameters
		[SerializeField]
		[Range(0f, 1f)]
		private float _swipeThreshold = 0.5f;	// TODO: rename

		[SerializeField]
		private UICarouselScrollLayer[] _layers;
		#endregion

		#region Getters
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
