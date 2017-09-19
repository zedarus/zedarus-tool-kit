using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit
{
	public class DelayedCall : MonoBehaviour
	{
		public void Init(System.Action callback, float delay, bool useRealtime, bool dontDestroyOnLoad)
		{
			if (dontDestroyOnLoad)
			{
				DontDestroyOnLoad(gameObject);
			}

			StartCoroutine(Delay(delay, callback, useRealtime));
		}

		private IEnumerator Delay(float delay, System.Action callback, bool useRealtime)
		{
			if (useRealtime)
			{
				yield return new WaitForSecondsRealtime(delay);
			}
			else
			{
				yield return new WaitForSeconds(delay);
			}

			if (callback != null)
			{
				callback();
				callback = null;
			}

			Destroy(gameObject);
		}

		public static DelayedCall Create(System.Action callback, float delay, bool useRealtime = true, bool dontDestroyOnLoad = false, string customName = null)
		{
			GameObject go = new GameObject((customName == null) ? "Delayed Call" : customName);
			DelayedCall call = go.AddComponent<DelayedCall>();
			call.Init(callback, delay, useRealtime, dontDestroyOnLoad);
			return call;
		}
	}
}
