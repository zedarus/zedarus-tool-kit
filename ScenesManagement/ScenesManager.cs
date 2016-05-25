using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.ScenesManagement
{
	public class ScenesManager : SimpleSingleton<ScenesManager>
	{
		#region Properties
		private string _nextScene = null;
		private string _currentScene = null;
		private string _defaultScene = null;
		private string _loadingScene = null;
		private string _previousScene = null;
		private bool _useLoadingScene = false;
		#endregion

		private Dictionary<string, Dictionary<object, object>> _params = new Dictionary<string, Dictionary<object, object>>();
		
		#region General Scene Switch Methods
		public virtual void ShowScene(string sceneName, params object[] data) 
		{
			_previousScene = _currentScene;
			_currentScene = _nextScene = sceneName;

			if (_params.ContainsKey(sceneName))
			{
				_params[sceneName].Clear();
			}
			else
			{
				_params[sceneName] = new Dictionary<object, object>();
			}

			if (data != null && data.Length > 0)
			{
				if (data.Length % 2 == 0)
				{
					for (int i = 0; i < data.Length; i += 2)
					{
						_params[sceneName].Add(data[i], data[i + 1]);	
					}
				}
				else
				{
					throw new UnityException("Each scene parameter must have a name pair");
				}
			}

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

		#region Parameters
		public T GetSceneParam<T>(string sceneName, object paramName, T defaultValue = default(T))
		{
			if (_params.ContainsKey(sceneName))
			{
				if (_params[sceneName].ContainsKey(paramName))
				{
					return (T)_params[sceneName][paramName];
				}
				else
				{
					return defaultValue;
				}
			}
			else
			{
				return defaultValue;
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
