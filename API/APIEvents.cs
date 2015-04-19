using UnityEngine;
using System.Collections;
using Zedarus.ToolKit.Events;

namespace Zedarus.ToolKit.API
{
	public class APIEvents
	{
		private const int SHIFT = 10900;
		public const int UploadScore = SHIFT + 1;
		public const int UploadAchievements = SHIFT + 2;
		public const int CurrencyPurchaseSuccess = SHIFT + 3;

		static public void Register()
		{
			EventManager.RegisterEvent(UploadScore);
			EventManager.RegisterEvent(UploadAchievements);
		}
	}
}
