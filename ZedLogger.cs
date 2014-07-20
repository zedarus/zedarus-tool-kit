using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit
{
	public enum LoggerContext
	{
		iCloud,
		General
	}

	public class ZedLogger : SimpleSingleton<ZedLogger>
	{
		private bool _developmentMode = true;

		static public void Log(object message, LoggerContext context = LoggerContext.General)
		{
			if (ContextIsBlocked(context))
				return;
		
			string prefix = GetPrefixForContext(context);
			Debug.Log(prefix + message);
		}
	
		static public void LogWarning(object message, LoggerContext context = LoggerContext.General)
		{
			if (ContextIsBlocked(context))
				return;
		
			string prefix = GetPrefixForContext(context);
			Debug.LogWarning(prefix + message);
		}
	
		static public void LogError(object message, LoggerContext context = LoggerContext.General)
		{
			if (ContextIsBlocked(context))
				return;
		
			string prefix = GetPrefixForContext(context);
			Debug.LogError(prefix + message);
		}
	
		static private bool ContextIsBlocked(LoggerContext context)
		{
			if (!ZedLogger.Instance.DevelopmentMode)
				return true;
		
			switch (context)
			{
				case LoggerContext.iCloud:
					return false;
				default:
					return false;
			}
		}
	
		static private string GetPrefixForContext(LoggerContext context)
		{
			switch (context)
			{
				case LoggerContext.iCloud:
					return "iCloud: ";
				default:
					return "";
			}
		}

		public void DisableDevelopmentMode()
		{
			_developmentMode = false;
		}

		public bool DevelopmentMode
		{
			get { return _developmentMode; }
		}
	}
}

