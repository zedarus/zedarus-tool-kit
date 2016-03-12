using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.UI
{
	public class UIGenericPopup : UIPopup
	{
		#region Parameters
		[Header("Basic Elements")]
		[SerializeField] 
		private LayoutGroup _layoutGroup;
		[SerializeField]
		private Text _header;
		[SerializeField]
		private Text _message;
		[SerializeField]
		private UIGenericPopupButton _button;
		#endregion

		#region Properties
		private List<UIGenericPopupButton> _buttons;
		#endregion

		#region Initialization
		public override void Init()
		{
			base.Init();

			_buttons = new List<UIGenericPopupButton>();
			if (_button != null)
			{
				_button.ClosePopup += OnClosePopupRequest;
				_button.Init();
				_buttons.Add(_button);
			}
		}

		protected override void Cleanup()
		{
			base.Cleanup();

			CleanUpButtons();
			if (_button != null)
			{
				_button.ClosePopup -= OnClosePopupRequest;
			}
		}
		#endregion

		#region Controls
		override protected void ProcessCustomData(IUIScreenData customData) 
		{
			base.ProcessCustomData(customData);

			bool dataExists = customData != null;

			CleanUpButtons();

			_header.gameObject.SetActive(dataExists);
			_message.gameObject.SetActive(dataExists);
			_button.gameObject.SetActive(dataExists);

			if (dataExists)
			{
				UIGenericPopupData data = (UIGenericPopupData)customData;

				SetText(_header, data.Header);
				SetText(_message, data.Message);

				SetupButtons(data.Buttons);

				if (data.Width > 0f)
				{
					RectTransform rt = _layoutGroup.GetComponent<RectTransform>();
					if (rt != null)
					{
						rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, data.Width);
					}
				}
			}
		}
		#endregion

		#region Event Handlers
		private void OnClosePopupRequest()
		{
			Close();
		}
		#endregion

		#region Helpers
		private void SetText(Text text, string content)
		{
			if (content != null)
			{
				text.text = content;
				text.gameObject.SetActive(true);
			}
			else
				text.gameObject.SetActive(false);
		}

		private void SetupButtons(UIGenericPopupButtonData[] buttons)
		{
			_button.gameObject.SetActive(buttons.Length > 0);

			if (buttons.Length > 0)
			{
				_button.Reset();

				while (_buttons.Count < buttons.Length)
				{
					UIGenericPopupButton newButton = Instantiate<UIGenericPopupButton>(_button);
					newButton.transform.parent = _button.transform.parent;
					newButton.transform.localScale = Vector3.one;
					newButton.Init();
					newButton.Reset();
					newButton.ClosePopup += OnClosePopupRequest;
					_buttons.Add(newButton);
				}

				for (int i = 0; i < buttons.Length; i++)
				{
					_buttons[i].ProcessCustomData(buttons[i]);
				}
			}
		}

		private void CleanUpButtons()
		{
			for (int i = _buttons.Count - 1; i > 0; i--)
			{
				_buttons[i].ClosePopup -= OnClosePopupRequest;
				Destroy(_buttons[i].gameObject);
			}
		}
		#endregion
	}
}
