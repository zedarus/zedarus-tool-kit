using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.API;

namespace Zedarus.ToolKit.Localisation
{
	public class LocalisationManager : SimpleSingleton<LocalisationManager>
	{
		#region Settings
		private const string DefaultPageName = "General";
		#endregion

		#region Properties
		private Dictionary<int, string> _pages;
		#endregion

		#region Init
		public LocalisationManager()
		{
			_pages = new Dictionary<int, string>();
			#if ZTK_LOC_M2H
			LanguageCode localLang = Language.LanguageNameToCode(Application.systemLanguage);
			APIManager.Instance.Analytics.LogSystemLanguage(localLang.ToString());
			#endif
		}
		#endregion

		#region Setup
		public void AddPage(int id, string name)
		{
			_pages.Add(id, name);
		}
		#endregion
		
		#region Localizing
		public void Localize() 
		{
			LocaliseObject[] localizeObjects = GameObject.FindObjectsOfType<LocaliseObject>();
			foreach (LocaliseObject o in localizeObjects)
				o.Localise(this);

			//#if NGUI
			//UIRoot.Broadcast("Localize", this);
			//#endif
		}

		public void LocalizeGameObject(GameObject gameObject) 
		{
			gameObject.BroadcastMessage("Localize", this, SendMessageOptions.DontRequireReceiver);
		}

		public string Get(string word, int page) 
		{
			#if ZTK_LOC_M2H
			return Language.Get(word, ConvertPageIDToStringValue(page));
			#else
			return "no localisation plugin";
			#endif
		}

		public string Get(string word) 
		{
			#if ZTK_LOC_M2H
			return Language.Get(word);
			#else
			return "no localisation plugin";
			#endif
		}

		#if ZTK_LOC_M2H
		public string GetForLanguage(string word, int page, LanguageCode language)
		{
			LanguageCode currentLanguage = Language.CurrentLanguage();
			ChangeLanguage(language);
			string s = Get(word, page);
			ChangeLanguage(currentLanguage);
			return s;
		}
		#endif
		#endregion
		
		#region Helpers
		private string ConvertPageIDToStringValue(int page) 
		{
			if (_pages.ContainsKey(page))
				return _pages [page];
			else
				return DefaultPageName;
		}

		public void ChangeLanguage(string languageCode) 
		{
			#if ZTK_LOC_M2H
			Language.SwitchLanguage(languageCode);
			APIManager.Instance.Analytics.LogLanguageChange(languageCode.ToLower());
			#endif
		}

		#if ZTK_LOC_M2H
		public void ChangeLanguage(LanguageCode languageCode) 
		{
			Language.SwitchLanguage(languageCode);
			APIManager.Instance.Analytics.LogLanguageChange(languageCode.ToString().ToLower());
		}
		#endif
		#endregion
	}
}
