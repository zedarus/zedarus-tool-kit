using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UserInterface
{
	public enum MoveDirection
	{
		Up,
		Down,
		Left,
		Right
	}

	public class UIMoveByAnimation : UIElementAnimation
	{
		#region Parameters
		[SerializeField] private float _distance = 0f;
		[SerializeField] private MoveDirection _direction = MoveDirection.Up;

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
			iTween.MoveTo(gameObject, iTween.Hash("position", ShowPosition, "islocal", true, "time", ShowDuration));
			base.Show();
		}
		
		public override void Hide()
		{
			iTween.MoveTo(gameObject, iTween.Hash("position", HidePosition, "islocal", true, "time", HideDuration));
			base.Hide();
		}
		
		public override void Reset()
		{
			_transform.localPosition = HidePosition;
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
			get { return _finalPosition + OppositeDirectionVector * _distance; }
		}

		private Vector3 OppositeDirectionVector
		{
			get 
			{
				switch (_direction)
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
		}
		#endregion
	}
}
