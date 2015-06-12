using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.Helpers
{
	public class SimpleTimer : MonoBehaviour
	{
		public event Action OnTimerEnd;
		
		static public SimpleTimer CreateTimer()
		{
			GameObject g = new GameObject("Simple Timer");
			return g.AddComponent<SimpleTimer>();
		}
		
		public void OnDestory()
		{
			OnTimerEnd = null;
			CancelInvoke();
		}
		
		public void StartTimer(float delay)
		{
			Invoke("StopTimer", delay);
		}
		
		private void StopTimer()
		{
			if (OnTimerEnd != null)
				OnTimerEnd();

			OnTimerEnd = null;
			
			Destroy(gameObject);
		}
	}
}
