using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UserInterface
{
	#if !ZTK_DISABLE_NGUI
	public enum MoveDirection
	{
		Up,
		Down,
		Left,
		Right
	}
	
	[AddComponentMenu("ZTK/UI/Move By Animation")]
	public class UIMoveByAnimation : UIElementAnimation
	{
		#region Parameters
		[SerializeField] private float _showDistance = 0f;
		[SerializeField] private MoveDirection _showDirection = MoveDirection.Up;
		[SerializeField] private iTween.EaseType _showEaseType = iTween.EaseType.easeOutExpo;
		[SerializeField] private float _hideDistance = 0f;
		[SerializeField] private MoveDirection _hideDirection = MoveDirection.Down;
		[SerializeField] private iTween.EaseType _hideEaseType = iTween.EaseType.easeOutExpo;

		private Transform _transform;
		private Vector3 _finalPosition;
		#endregion
		
		public override void Init()
		{
			_transform = transform;
			_finalPosition = _transform.localPosition;
			base.Init();
		}
		
		#region Controls
		public override void Show()
		{
			iTween.MoveTo(gameObject, iTween.Hash("position", ShowPosition, "islocal", true, "easetype", _showEaseType, "time", ShowDuration));
			base.Show();
		}
		
		public override void Hide()
		{
			iTween.MoveTo(gameObject, iTween.Hash("position", HidePosition, "islocal", true, "easetype", _hideEaseType, "time", HideDuration));
			base.Hide();
		}
		
		public override void Reset()
		{
			if (_transform != null)
				_transform.localPosition = ResetPosition;
			base.Reset();
		}
		#endregion

		#region Helpers
		private Vector3 ShowPosition
		{
			get { return _finalPosition; }
		}

		private Vector3 HidePosition
		{
			get { return _finalPosition + VectorFromDirection(_hideDirection) * _hideDistance; }
		}

		private Vector3 ResetPosition
		{
			get { return _finalPosition + OppositeVectorFromDirection(_showDirection) * _showDistance; }
		}

		private Vector3 VectorFromDirection(MoveDirection direction)
		{
			switch (direction)
			{
				case MoveDirection.Up:
					return Vector3.up;
				case MoveDirection.Down:
					return Vector3.down;
				case MoveDirection.Left:
					return Vector3.left;
				case MoveDirection.Right:
					return Vector3.right;
				default:
					return Vector3.zero;
			}
		}

		private Vector3 OppositeVectorFromDirection(MoveDirection direction)
		{
			switch (direction)
			{
				case MoveDirection.Up:
					return Vector3.down;
				case MoveDirection.Down:
					return Vector3.up;
				case MoveDirection.Left:
					return Vector3.right;
				case MoveDirection.Right:
					return Vector3.left;
				default:
					return Vector3.zero;
			}
		}
		#endregion
	}
	#endif
}
