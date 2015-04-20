using UnityEngine;
using System.Collections;
using Zedarus.ToolKit.API;
using Zedarus.ToolKit.Data;

namespace Zedarus.ToolKit
{
	public class AppController : MonoBehaviour
	{
		#region Properties
		static private bool initialized = false;
		#endregion

		#region Unity Methods
		private void Start()
		{
			if (!initialized)
				Init();

			Launch();
			Destroy(gameObject);
		}
		#endregion

		#region Init
		private void Init()
		{
			InitEvents();
			InitAPI();
			APIManager.Instance.Init();

			InitGameData();
			InitPlayerData("default.dat");
			
			initialized = true;
		}

		protected virtual void InitEvents()
		{
			
		}

		protected virtual void InitAPI()
		{
			
		}

		protected virtual void InitGameData()
		{

		}

		protected virtual void InitPlayerData(string filename, APIState customAPIState = null)
		{
			DataManager.Instance.Load(filename);
			if (customAPIState == null)
			{
				DataManager.Instance.Player.AddModel<APIState>();
				customAPIState = DataManager.Instance.Player.GetModel<APIState>();
			}
			APIManager.Instance.UseState(customAPIState);
		}

		protected virtual void Launch()
		{
			
		}
		#endregion
	}
}
