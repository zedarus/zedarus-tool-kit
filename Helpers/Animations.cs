using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Helpers
{
	public class Animations
	{
		#region Animation
		public static IEnumerator Move(Transform pivot, Vector3 to, AnimationCurve curve, bool local)
		{
			Vector3 from = local ? pivot.transform.localPosition : pivot.transform.position;
			yield return Move(pivot, from, to, curve, local);
		}

		public static IEnumerator Move(Transform pivot, Vector3 from, Vector3 to, AnimationCurve curve, bool local)
		{
			float timer = 0f;
			float duration = curve.keys[curve.keys.Length - 1].time;

			while (timer <= duration)
			{
				timer += Time.deltaTime;

				if (timer > duration)
					timer = duration;

				if (local)
					pivot.localPosition = Vector3.LerpUnclamped(from, to, curve.Evaluate(timer));
				else
					pivot.position = Vector3.LerpUnclamped(from, to, curve.Evaluate(timer));

				yield return new WaitForEndOfFrame();
			}
		}
		#endregion
	}
}
