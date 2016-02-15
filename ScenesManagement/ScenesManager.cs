using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.ScenesManagement
{
	public class ScenesManager : SimpleSingleton<ScenesManager>
	{
		#region Parameters
		private string _nextScene = null;
		private string _currentScene = null;
		private string _defaultScene = null;
		private string _loadingScene = null;
		private string _previousScene = null;
		private bool _useLoadingScene = false;
		#endregion
		
		#region General Scene Switch Methods
		public virtual void ShowScene(string sceneName) 
		{
			_previousScene = _currentScene;
			_currentScene = _nextScene = sceneName;

			if (_useLoadingScene)
				SceneManager.LoadScene(LoadingSceneName);
			else
			{
				Resources.UnloadUnusedAssets();
				SceneManager.LoadScene(_nextScene);
			}
		}

		public virtual void ShowPreviousScene() 
		{
			if (_previousScene != null)
				ShowScene(_previousScene);
			else 
			{
				ZedLogger.Log("No previous scene is set, switching to default screen instead.");
				ShowDefaultScene();
			}
		}
		#endregion

		public void UseLoadingScene()
		{
			_useLoadingScene = true;
		}

		protected void ShowDefaultScene()
		{
			ShowScene(DefaultSceneName);
		}

		#region Getters
		public string DefaultSceneName
		{
			get { return _defaultScene; }
			set { _defaultScene = value; }
		}

		public string LoadingSceneName
		{
			get { return _loadingScene; }
			set { _loadingScene = value; }
		}
		#endregion
		
		#region Scenes Loading
		private AsyncOperation LoadScene(string sceneName) 
		{
			AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
			asyncOperation.allowSceneActivation = false;
			return asyncOperation;
		}

		public AsyncOperation LoadNextScene() 
		{
			if (_nextScene != null) 
			{
				AsyncOperation asyncOperation = LoadScene(_nextScene);
				_nextScene = null;
				return asyncOperation;
			} 
			else 
			{
				ZedLogger.LogWarning("No next scene is set!");
				return null;
			}
		}
		#endregion
	}
}
