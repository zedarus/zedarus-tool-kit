using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.UI
{
	public struct UIGenericPopupData : IUIScreenData
	{
		#region Properties
		private string _header;
		private string _message;
		private float _width;
		private float _timeout;
		private UIGenericPopupButtonData[] _buttons;
		#endregion

		#region Init
		public UIGenericPopupData(string header, string message, params UIGenericPopupButtonData[] buttons)
		{
			_header = header;
			_message = message;
			_width = 0;
			_timeout = 0;
			_buttons = buttons;
		}

		public UIGenericPopupData(string header, string message, float timeout, float width = 0, params UIGenericPopupButtonData[] buttons)
		{
			_header = header;
			_message = message;
			_width = width;
			_timeout = timeout;
			_buttons = buttons;
		}

		public void Clear()
		{
			_header = null;
			_message = null;

			for (int i = 0; i < _buttons.Length; i++)
			{
				_buttons[i].Clear();
			}

			_buttons = null;
		}
		#endregion

		#region Getters
		public string Header
		{
			get { return _header; }
		}

		public string Message
		{
			get { return _message; }
		}

		public float Width
		{
			get { return _width; }
		}

		public float Timeout
		{
			get { return _timeout; }
		}

		public UIGenericPopupButtonData[] Buttons
		{
			get { return _buttons; }
		}
		#endregion
	}

	public struct UIGenericPopupButtonData : IUIScreenData
	{
		#region Properties
		private string _label;
		private Color? _color;
		private System.Action _callback;
		private bool _closePopupOnPress;
		#endregion

		#region Init
		public UIGenericPopupButtonData(string label)
		{
			_label = label;
			_callback = null;
			_color = null;
			_closePopupOnPress = true;
		}

		public UIGenericPopupButtonData(string label, System.Action callback, bool closePopupOnPress = true)
		{
			_label = label;
			_callback = callback;
			_color = null;
			_closePopupOnPress = closePopupOnPress;
		}

		public UIGenericPopupButtonData(string label, System.Action callback, Color color, bool closePopupOnPress = true)
		{
			_label = label;
			_callback = callback;
			_color = color;
			_closePopupOnPress = closePopupOnPress;
		}

		public UIGenericPopupButtonData(string label, System.Action callback, byte red, byte green, byte blue, byte alpha = 255, bool closePopupOnPress = true)
		{
			_label = label;
			_callback = callback;
			_color = new Color32(red, green, blue, alpha);
			_closePopupOnPress = closePopupOnPress;
		}

		public void Clear()
		{
			_label = null;
			_callback = null;
			_color = null;
		}
		#endregion

		#region Getters
		public string Label
		{
			get { return _label; }
		}

		public System.Action Callback
		{
			get { return _callback; }
		}

		public Color? Color
		{
			get { return _color; }
		}

		public bool ClosePopupOnPress
		{
			get { return _closePopupOnPress; }
		}
		#endregion
	}
}
