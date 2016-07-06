using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Zedarus.ToolKit.Audio;

namespace Zedarus.ToolKit.UI.Elements
{
	public abstract class UIButtonAudio : MonoBehaviour, IPointerClickHandler
	{
		#region Parameters
		[Header("Leave fields blank to use default IDs")]
		[SerializeField]
		private string _onClick = null;
		#endregion

		#region IPointerClickHandler
		public void OnPointerClick(PointerEventData eventData)
		{
			PlaySound(_onClick, DefaultClickSound);
		}
		#endregion

		#region Helpers
		private void PlaySound(string id, string defaultID)
		{
			if (Audio != null)
			{
				if (string.IsNullOrEmpty(id.Trim()))
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
		protected abstract AudioController Audio { get; }
		#endregion
	}
}
