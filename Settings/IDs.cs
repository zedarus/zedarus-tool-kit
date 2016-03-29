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
			public const int RemoteDataReceived = EVENT_OFFSET + 4;
			public const int GameDataUpdated = EVENT_OFFSET + 5;
		}

		private static bool _initialized = false;
		public static void Init()
		{
			if (!_initialized)
			{
				EventManager.RegisterEvent(Events.DisableMusicDuringAd);
				EventManager.RegisterEvent(Events.EnableMusicAfterAd);
				EventManager.RegisterEvent(Events.DisableAds);
				EventManager.RegisterEvent(Events.RemoteDataReceived);
				EventManager.RegisterEvent(Events.GameDataUpdated);
				_initialized = true;
			}
		}
	}
}

