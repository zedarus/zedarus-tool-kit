using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.ScenesManagement
{
	public class LoadingScene : MonoBehaviour
	{
		#region Parameters
		public float minimumLoadingDuration = 0.5f;
		private AsyncOperation _loadingOperation;
		private float _loadingTimer = 0.0f;
		#endregion
		
		#region Unity Methods
		private void Awake() 
		{
			Resources.UnloadUnusedAssets();
		}
		
		private void Start() 
		{
			StartLoading();
		}
		
		private void Update() 
		{
			UpdateLoading(Time.deltaTime);
		}
		#endregion

		#region Loading
		protected virtual void StartLoading()
		{
			_loadingTimer = 0.0f;
			_loadingOperation = ScenesManager.Instance.LoadNextScene();
		}

		protected virtual void UpdateLoading(float deltaTime)
		{
			_loadingTimer += deltaTime;
			
			if (_loadingOperation != null) 
			{
				if (_loadingOperation.progress >= 0.9f && _loadingTimer >= minimumLoadingDuration) 
					_loadingOperation.allowSceneActivation = true;
			}
		}
		#endregion
	}
}
