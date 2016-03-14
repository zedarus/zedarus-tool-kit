using System;
using Zedarus.ToolKit.Events;

namespace Zedarus.ToolKit.Settings
{
	public class IDs
	{
		public const int EVENT_OFFSET = 1000;

		public struct Events
		{
			public const int DisableMusicDuringAd = EVENT_OFFSET + 1;
			public const int EnableMusicAfterAd = EVENT_OFFSET + 2;
			public const int DisableAds = EVENT_OFFSET + 3;
		}

		private static bool _initialized = false;
		public static void Init()
		{
			if (!_initialized)
			{
				EventManager.RegisterEvent(Events.DisableMusicDuringAd);
				EventManager.RegisterEvent(Events.EnableMusicAfterAd);
				EventManager.RegisterEvent(Events.DisableAds);
				_initialized = true;
			}
		}
	}
}

