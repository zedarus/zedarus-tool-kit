using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Zedarus.ToolKit.Audio;

namespace Zedarus.ToolKit.UI.Elements
{
	public abstract class UIButtonAudio : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
	{
		#region Parameters
		[Header("Leave fields blank to use default IDs")]
		[SerializeField]
		private string _onClick = null;
		[SerializeField]
		private string _onPointerDown = null;
		#endregion

		#region Properties
		private Button _button = null;
		#endregion

		#region Unity Methods
		private void Start()
		{
			_button = GetComponent<Button>();
		}
		#endregion

		#region IPointerClickHandler
		public void OnPointerClick(PointerEventData eventData)
		{
			if (_button != null && _button.interactable)
			{
				PlaySound(_onClick, DefaultClickSound);
			}
		}
		#endregion

		#region IPointerDownHandler
		public void OnPointerDown(PointerEventData eventData)
		{
			if (_button != null && _button.interactable)
			{
				PlaySound(_onPointerDown, DefaultOnPointDownSound);
			}
		}
		#endregion

		#region Helpers
		private void PlaySound(string id, string defaultID)
		{
			if (Audio != null)
			{
				if (id != null)
				{
					id = id.Trim();
				}

				if (string.IsNullOrEmpty(id))
				{
					Audio.PlaySound(defaultID);
				}
				else
				{
					Audio.PlaySound(id);
				}
			}
		}
		#endregion

		#region Getters
		protected abstract string DefaultClickSound { get; }
		protected abstract string DefaultOnPointDownSound { get; }
		protected abstract AudioController Audio { get; }
		#endregion
	}
}
