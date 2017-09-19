using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.ScenesManagement
{
	public class FadeScreenTracker : MonoBehaviour
	{
		public void Init(Animator animator, int stateHash, System.Action callback)
		{
			StartCoroutine(Track(animator, stateHash, callback));
		}

		private IEnumerator Track(Animator animator, int stateHash, System.Action callback)
		{
			float timer = 0;
			bool state = false;

			while (!state && timer <= 5f)
			{
				state = animator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash;
				timer += Time.unscaledDeltaTime;
				yield return new WaitForEndOfFrame();
			}

			if (state && callback != null)
			{
				callback();		
				callback = null;
			}
		}
	}

	public class ScenesManager : SimpleSingleton<ScenesManager>
	{
		#region Properties
		private string _nextScene = null;
		private string _currentScene = null;
		private string _defaultScene = null;
		private string _loadingScene = null;
		private string _previousScene = null;
		private bool _useLoadingScene = false;
		private Animator _fadeScreen = null;
		private FadeScreenTracker _fadeScreenTracker = null;
		private object[] _cachedData = null;

		private int FadeInTrigger;
		private int FadeOutTrigger;
		#endregion

		private Dictionary<string, Dictionary<object, object>> _params = new Dictionary<string, Dictionary<object, object>>();
		
		#region General Scene Switch Methods
		private void OnFadeOutFinished()
		{
			GameObject.Destroy(_fadeScreenTracker.gameObject);
			_fadeScreen = null;
			ShowScene(_nextScene, _cachedData);
		}

		public virtual void ShowScene(string sceneName, params object[] data) 
		{
			_cachedData = null;
			_previousScene = _currentScene;
			_currentScene = _nextScene = sceneName;

			if (_fadeScreen != null)
			{
				_cachedData = data;

				GameObject go = new GameObject("Fade Screen Tracker");
				_fadeScreenTracker = go.AddComponent<FadeScreenTracker>();
				_fadeScreenTracker.Init(_fadeScreen, FadeOutTrigger, OnFadeOutFinished);

				_fadeScreen.ResetTrigger(FadeInTrigger);
				_fadeScreen.ResetTrigger(FadeOutTrigger);
				_fadeScreen.SetTrigger(FadeOutTrigger);
			}
			else
			{
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
				{
					SceneManager.LoadScene(_loadingScene);
				}
				else
				{
					Resources.UnloadUnusedAssets();
					SceneManager.LoadScene(_nextScene);
				}
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

		/// <summary>
		/// Call this at the beginning of the scene to fade in.
		/// </summary>
		/// <param name="fadeScreen">Fade screen.</param>
		public void UseFadeScreen(Animator fadeScreen, float delay = 0f, string fadeInTrigger = null, string fadeOutTrigger = null)
		{
			if (fadeScreen)
			{
				if (fadeInTrigger != null)
				{
					FadeInTrigger = Animator.StringToHash(fadeInTrigger); 
				}
				else
				{
					FadeInTrigger = Animator.StringToHash("FadeIn"); 
				}

				if (fadeOutTrigger != null)
				{
					FadeOutTrigger = Animator.StringToHash(fadeOutTrigger); 
				}
				else
				{
					FadeOutTrigger = Animator.StringToHash("FadeOut"); 
				}

				_fadeScreen = fadeScreen;

				if (delay > 0)
				{
					DelayedCall.Create(UseFadeScreen, delay, false);
				}
				else
				{
					UseFadeScreen();
				}
			}
			else
			{
				_fadeScreen = null;
			}
		}

		private void UseFadeScreen()
		{
			if (_fadeScreen != null)
			{
				_fadeScreen.ResetTrigger(FadeInTrigger);
				_fadeScreen.ResetTrigger(FadeOutTrigger);
				_fadeScreen.SetTrigger(FadeInTrigger);
			}
		}

		public void UseLoadingScene(string loadingSceneName)
		{
			_loadingScene = loadingSceneName;
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
