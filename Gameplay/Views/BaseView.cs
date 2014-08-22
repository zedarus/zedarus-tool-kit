using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Gameplay.Views
{
	public class BaseView : MonoBehaviour 
	{
		protected Transform _transform;
		protected Vector3 _startPosition;
		private bool _cached = false;
		
		protected void CacheTransform()
		{
			if (!_cached) 
			{
				_transform = transform;
				_startPosition = _transform.localPosition;
				_cached = true;
			}
		}

		public virtual void Reset() {}
		
		public virtual void Draw(float deltaTime) {}
	}
}
