using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Localisation
{
	public class LocaliseObject : MonoBehaviour
	{
		public virtual void Localise(LocalisationManager localisationManager)
		{
			Debug.Log("Localise");
		}
	}
}
