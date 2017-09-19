using System;
using Zedarus.ToolKit.API;
using Zedarus.ToolKit.Helpers.Modules;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Player;

namespace Zedarus.ToolKit.Helpers
{
	public class AppHelpers
	{
		#region Properties
		private PromoHelper _promo;
		#endregion

		#region Init
		public AppHelpers(GameData gameDataRef, APIManager api)
		{
			_promo = new PromoHelper(gameDataRef, api);
		}
		#endregion

		#region Getters
		public PromoHelper Promo
		{
			get { return _promo; }
		}
		#endregion
	}
}

