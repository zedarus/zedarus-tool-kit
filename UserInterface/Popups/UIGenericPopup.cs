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
		private UIGenericPopupButton[] _originalButtons;
		#endregion

		#region Properties
		private List<UIGenericPopupButton> _buttons;
		private IEnumerator _closingRoutine = null;
		#endregion

		#region Initialization
		public override void Init()
		{
			base.Init();

			_buttons = new List<UIGenericPopupButton>();
			if (_originalButtons != null)
			{
				foreach (UIGenericPopupButton button in _originalButtons)
				{
					button.ClosePopup += OnClosePopupRequest;
					button.Init();
				}

				_buttons.AddRange(_originalButtons);
			}
		}

		protected override void Cleanup()
		{
			base.Cleanup();

			CleanUpButtons();
			if (_originalButtons != null)
			{
				foreach (UIGenericPopupButton button in _originalButtons)
				{
					button.ClosePopup -= OnClosePopupRequest;
				}
			}
		}
		#endregion

		#region Controls
		override protected void ProcessCustomData(IUIScreenData customData) 
		{
			if (_closingRoutine != null)
			{
				StopCoroutine(_closingRoutine);
				_closingRoutine = null;
			}

			base.ProcessCustomData(customData);

			bool dataExists = customData != null;

			CleanUpButtons();

			_header.gameObject.SetActive(dataExists);
			_message.gameObject.SetActive(dataExists);

			foreach (UIGenericPopupButton button in _originalButtons)
			{
				button.gameObject.SetActive(dataExists);
			}

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

				if (data.Timeout > 0f)
				{
					CloseWithDelay(data.Timeout);
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
			for (int i = 0; i < _originalButtons.Length; i++)
			{
				_originalButtons[i].Reset();
				_originalButtons[i].gameObject.SetActive(buttons.Length > i);
			}

			if (buttons.Length > 0 && _originalButtons.Length > 0)
			{
				while (_buttons.Count < buttons.Length)
				{
					UIGenericPopupButton newButton = Instantiate<UIGenericPopupButton>(_originalButtons[0]);
					newButton.transform.SetParent(_originalButtons[0].transform.parent, false);
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
			for (int i = _buttons.Count - 1; i > _originalButtons.Length - 1; i--)
			{
				_buttons[i].ClosePopup -= OnClosePopupRequest;

				if (_buttons[i] != null && _buttons[i].gameObject != null)
					Destroy(_buttons[i].gameObject);

				_buttons.RemoveAt(i);
			}
		}

		private void CloseWithDelay(float delay)
		{
			if (_closingRoutine != null)
			{
				StopCoroutine(_closingRoutine);
				_closingRoutine = null;
			}

			_closingRoutine = CloseWithDelayRoutine(delay);
			StartCoroutine(_closingRoutine);
		}

		private IEnumerator CloseWithDelayRoutine(float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			Close();
		}
		#endregion
	}
}
