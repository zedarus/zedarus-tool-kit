using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Events;
using Zedarus.ToolKit.Settings;

namespace Zedarus.ToolKit.Localisation
{
	public class LocalisationManager : SimpleSingleton<LocalisationManager>
	{
		#region Properties
		private string _firstPage = null;
		private bool _ready = false;
		#endregion

		#region Controls
		public void Init()
		{
			if (!_ready)
			{
				EventManager.SendEvent<string>(IDs.Events.SetLanguage, Language.CurrentLanguage().ToString());
				_ready = true;
			}
		}

		public void ChangeLanguage(LanguageCode language, bool notifyAllObjects)
		{
			Language.SwitchLanguage(language);
			EventManager.SendEvent<string>(IDs.Events.SetLanguage, language.ToString());

			if (notifyAllObjects)
			{
				LocaliseText[] localiseComponents = GameObject.FindObjectsOfType<LocaliseText>();
				foreach (LocaliseText localiseComponent in localiseComponents)
				{
					localiseComponent.Localise();
				}
			}
		}

		public string Localise(string key)
		{
			return Localise(key, FirstPage);
		}

		public string Localise(string key, string page)
		{
			return Language.Get(key, page);
		}

		public string Localise(string key, object enumerablePage)
		{
			if (enumerablePage is System.Enum)
			{
				System.Type type = enumerablePage.GetType();
				if (type != null)
				{
					return Localise(key, type.Name);
				}
				else
					return null;
			}
			else
				return null;
		}

		public string Localise(object enumerable)
		{
			if (enumerable is System.Enum)
			{
				System.Type type = enumerable.GetType();
				if (type != null)
				{
					return Localise(System.Enum.GetName(type, enumerable), type.Name);
				}
				else
					return null;
			}
			else
				return null;
		}
		#endregion

		#region Helpers
		private string FirstPage
		{
			get 
			{
				if (_firstPage == null)
				{
					List<string> list = new List<string>(Language.Sheets.Keys);
					_firstPage = list[list.Count - 1];
					list.Clear();
					list = null;
				}

				return _firstPage;
			}
		}

		public bool Ready
		{
			get { return _ready; }
		}
		#endregion

		#if UNITY_EDITOR
		private Dictionary<string, List<string>> _data = new Dictionary<string, List<string>>();

		public bool HasData
		{
			get { return _data.Count > 0; }
		}

		public void UpdateData()
		{
			_data.Clear();

			Language.LoadAvailableLanguages();
			Language.SwitchLanguage(LanguageCode.EN);

			// TODO: this should be modular for different libs
			foreach (KeyValuePair<string, Dictionary<string, string>> sheet in Language.Sheets)
			{
				List<string> phrases = new List<string>();

				foreach (KeyValuePair<string, string> phrase in sheet.Value)
				{
					phrases.Add(phrase.Key);
				}

				_data.Add(sheet.Key, phrases);
			}
		}

		public string[] Sheets
		{
			get
			{
				List<string> list = new List<string>(_data.Keys);
				list.Reverse();
				return list.ToArray();
			}
		}

		public string[] GetPhrasesForSheet(string sheet)
		{
			if (_data.ContainsKey(sheet))
				return _data[sheet].ToArray();
			else
				return null;
		}
		#endif
	}
}
