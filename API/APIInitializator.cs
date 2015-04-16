using UnityEngine;
using System.Collections;
using Zedarus.Traffico.Data.PlayerData;

namespace Zedarus.ToolKit.API
{
	public class APIInitializator : MonoBehaviour
	{
		static public bool Initialized = false;
		
		private void Awake()
		{
			if (Initialized)
				Destroy(gameObject);
			else
			{
				APIManager.Instance.Init();
				PlayerDataManager.Instance.LoadOnStart();
				APIInitializator.Initialized = true;
				Destroy(gameObject);
			}
		}
	}
}
