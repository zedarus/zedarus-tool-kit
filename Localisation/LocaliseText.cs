using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Zedarus.ToolKit.Events;

namespace Zedarus.ToolKit.Localisation
{
	public abstract class LocaliseText : MonoBehaviour
	{
		#region Properties
		[SerializeField] private Text _textLabel;
		[SerializeField] private string _page;
		[SerializeField] private string _phrase;
		[SerializeField] private bool _localiseAtStart = false;

		private string _localisedString = null;
		private object[] _cachedArgs = null;
		#endregion

		#region Unity Methods
		private void Start()
		{
			if (_textLabel == null)
				_textLabel = GetComponent<Text>();

			if (_localiseAtStart)
			{
				StartCoroutine(LocaliseWhenReady());
			}
		}
		#endregion

		#region Controls
		public void Localise()
		{
			if (_textLabel != null && _page != null && _phrase != null)
			{
				_localisedString = LocManagerRef.Localise(_phrase, _page);
				_textLabel.text = _localisedString;

				if (_cachedArgs != null)
					SetParametersValues(_cachedArgs);
			}
		}

		public void SetParametersValues(params object[] args)
		{
			if (args.Length > 0)
				_cachedArgs = args;

			if (_textLabel != null && _localisedString != null && _cachedArgs.Length > 0)
			{
				_textLabel.text = string.Format(_localisedString, _cachedArgs);
			}
		}
		#endregion

		#region Helpers
		private IEnumerator LocaliseWhenReady()
		{
			for (int i = 0; i < 100; i++)
			{
				if (LocManagerRef != null && LocManagerRef.Ready)
				{
					Localise();
				}
				else
				{
					yield return new WaitForSecondsRealtime(0.1f);
				}
			}
		}
		#endregion

		#region Getters
		protected abstract LocalisationManager LocManagerRef { get; }
		#endregion
	}
}

