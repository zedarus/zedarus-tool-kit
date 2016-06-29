using UnityEngine;
using System.Collections;
using Zedarus.ToolKit.ScenesManagement;

namespace Zedarus.ToolKit.ScenesManagement
{
	public class LoadingScene : MonoBehaviour
	{
		private const float minimumLoadingDuration = 0.01f;
		private AsyncOperation _loading;
		private float _loadingTimer = 0f;

		private void Awake()
		{
			Resources.UnloadUnusedAssets();
		}

		private void Start()
		{
			_loadingTimer = 0f;
			_loading = ScenesManager.Instance.LoadNextScene();
		}

		private void Update()
		{
			_loadingTimer += Time.unscaledDeltaTime;

			if (_loading != null)
			{
				if (_loading.progress >= 0.9f && _loadingTimer >= minimumLoadingDuration)
				{
					_loading.allowSceneActivation = true;
				}
			}
		}
	}
}
