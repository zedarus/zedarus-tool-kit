using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Player;
using Zedarus.ToolKit.UI;
using Zedarus.ToolKit.API;
using Zedarus.ToolKit.Localisation;

namespace Zedarus.ToolKit.Extentions
{
	public class ExtentionUIPopup : Extention
	{
		#region Properties
		private LocalisationManager _localisation;
		private string _genericPopupID;

		private Dictionary<int, object> _localisationKeys;
		private Dictionary<int, int> _colorKeys;
		#endregion

		#region Settings
		protected const int POPUP_HEADER = 1;
		protected const int POPUP_MESSAGE = 2;
		#endregion

		#region Init
		public ExtentionUIPopup(APIManager api, LocalisationManager localisationManager, string genericPopupID, object popupHeaderLocalisationID, object popupMessageLocalisationID) : base(api)
		{
			_localisation = localisationManager;
			_genericPopupID = genericPopupID;

			CreateLocalisationKey(POPUP_HEADER, popupHeaderLocalisationID);
			CreateLocalisationKey(POPUP_MESSAGE, popupMessageLocalisationID);
		}
		#endregion

		#region UI
		protected UIGenericPopupButtonData CreateButton(int labelKey, System.Action callback, int colorKey)
		{
			return CreateButton(Localise(labelKey), callback, colorKey);
		}

		protected UIGenericPopupButtonData CreateButton(string label, System.Action callback, int colorKey)
		{
			Debug.Log("Color: " + GetColor(colorKey));
			return new UIGenericPopupButtonData(label, callback, GetColor(colorKey));
		}

		protected void DisplayPopup(UIManager uiManager, params UIGenericPopupButtonData[] buttons)
		{
			DisplayPopup(uiManager, Localise(POPUP_HEADER), Localise(POPUP_MESSAGE), buttons);
		}

		protected void DisplayPopup(UIManager uiManager, string header, string message, params UIGenericPopupButtonData[] buttons)
		{
			uiManager.OpenPopup(_genericPopupID, new Zedarus.ToolKit.UI.UIGenericPopupData(
				header, message, buttons
			));
		}
		#endregion

		#region Colors
		protected void CreateColorKey(int key, int color)
		{
			if (_colorKeys == null)
			{
				_colorKeys = new Dictionary<int, int>();
			}

			if (!_colorKeys.ContainsKey(key))
			{
				_colorKeys.Add(key, color);
			}
			else
			{
				throw new UnityException("Color key already used");
			}
		}

		protected int GetColor(int key)
		{
			if (_colorKeys.ContainsKey(key))
			{
				return _colorKeys[key];
			}
			else
			{
				return 0;
			}
		}
		#endregion

		#region Localisation
		protected void CreateButtonKeys(int key, object labelLocalisationID, int color)
		{
			CreateLocalisationKey(key, labelLocalisationID);
			CreateColorKey(key, color);
		}

		protected void CreateLocalisationKey(int key, object value)
		{
			if (_localisationKeys == null)
			{
				_localisationKeys = new Dictionary<int, object>();
			}

			if (!_localisationKeys.ContainsKey(key))
			{
				_localisationKeys.Add(key, value);
			}
			else
			{
				throw new UnityException("Localisation key already used");
			}
		}

		protected string Localise(int key)
		{
			if (_localisation != null && _localisationKeys.ContainsKey(key))
			{
				if (_localisationKeys[key] != null)
				{
					return _localisation.Localise(_localisationKeys[key]);
				}
				else
				{
					return null;
				}
			}
			else
			{
				return "NO_LOCALISATION_KEY";
			}
		}
		#endregion
	}
}

