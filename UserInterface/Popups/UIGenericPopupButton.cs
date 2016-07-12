using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Zedarus.ToolKit.UI
{
	public class UIGenericPopupButton : MonoBehaviour
	{
		#region Parameters
		[SerializeField] 
		private Text _label;
		#endregion

		#region Properties
		private string _initialLabel;
		private System.Action _callback = null;
		private bool _closePopupOnPress = false;
		#endregion

		#region Events
		public event System.Action ClosePopup;
		#endregion

		#region Initialization
		public void Init()
		{
			_initialLabel = _label.text;
			ApplyColor(0);
		}

		public void Reset()
		{
			_label.text = _initialLabel;
		}
		#endregion

		#region Callbacks
		public void OnClick()
		{
			if (_callback != null)
				_callback();

			if (_closePopupOnPress)
			{
				if (ClosePopup != null)
					ClosePopup();
			}
		}
		#endregion

		#region Controls
		public virtual void ProcessCustomData(IUIScreenData customData)
		{
			if (customData != null)
			{
				UIGenericPopupButtonData data = (UIGenericPopupButtonData)customData;

				_label.text = data.Label;
				_callback = data.Callback;
				_closePopupOnPress = data.ClosePopupOnPress;

				if (data.ColorID > 0)
				{
					ApplyColor(data.ColorID);
				}
			}
		}
		#endregion

		#region Helpers
		protected virtual void ApplyColor(int colorID)
		{
			
		}
		#endregion
	}
}
