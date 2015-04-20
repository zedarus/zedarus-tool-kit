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
			LanguageCode localLang = Language.LanguageNameToCode(Application.systemLanguage);
			APIManager.Instance.Analytics.LogSystemLanguage(localLang.ToString());
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
			return Language.Get(word, ConvertPageIDToStringValue(page));
		}

		public string Get(string word) 
		{
			return Language.Get(word);
		}
		
		public string GetForLanguage(string word, int page, LanguageCode language)
		{
			LanguageCode currentLanguage = Language.CurrentLanguage();
			ChangeLanguage(language);
			string s = Get(word, page);
			ChangeLanguage(currentLanguage);
			return s;
		}
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
			Language.SwitchLanguage(languageCode);
			APIManager.Instance.Analytics.LogLanguageChange(languageCode.ToLower());
		}
		
		public void ChangeLanguage(LanguageCode languageCode) 
		{
			Language.SwitchLanguage(languageCode);
			APIManager.Instance.Analytics.LogLanguageChange(languageCode.ToString().ToLower());
		}
		#endregion
	}
}
