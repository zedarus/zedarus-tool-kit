using System;
using Zedarus.ToolKit.API;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Data.Game;
using Zedarus.ToolKit.Data.Player;

namespace Zedarus.ToolKit.Helpers.Modules
{
	public class AppHelperBase
	{
		#region Properties
		private GameData _gameDataRef = null;
		private APIManager _apiRef = null;
		#endregion

		#region Init
		public AppHelperBase(GameData gameDataRef, APIManager api)
		{
			_gameDataRef = gameDataRef;
			_apiRef = api;
		}
		#endregion

		#region Getters
		protected GameData GameData
		{
			get { return _gameDataRef; }
		}

		protected APIManager API
		{
			get { return _apiRef; }
		}
		#endregion
	}
}

