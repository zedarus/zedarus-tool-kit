using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

namespace Zedarus.ToolKit
{
	public class SimpleSingleton<T> where T : class
	{
		private static T _instance;
	
		public static T Instance
		{
			get
			{
				if (_instance == null)
					CreateInstance();
			
				return _instance;
			}
		}
		
		protected static void CreateInstance()
		{
			if (_instance == null)
			{
				Type t = typeof(T);
		
				/*
				// Ensure there are no public constructors...
				ConstructorInfo[] ctors = t.GetConstructors();
				if (ctors.Length > 0)
				{
					throw new InvalidOperationException(String.Format("{0} has at least one accesible ctor making it impossible to enforce singleton behaviour", t.Name));
				}
				*/
		
				// Create an instance via the private constructor
				_instance = (T)Activator.CreateInstance(t, true);
			}
		}
	}
}

