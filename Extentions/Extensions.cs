using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zedarus.ToolKit.Extentions
{
	public static class Extensions
	{
		public static void Shuffle<T>(this IList<T> list)
		{  
			System.Random rng = new System.Random();  
			int n = list.Count;  
			while (n > 1)
			{  
				n--;  
				int k = rng.Next(n + 1);  
				T value = list [k];  
				list [k] = list [n];  
				list [n] = value;  
			}  
		}

		/// <summary>
		/// Returns the first instance of the monobehaviour that is of the interface type T (casted to T)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gObj"></param>
		/// <returns></returns>
		public static T GetInterfaceInChildren<T>(this GameObject gObj)
		{
			if (!typeof(T).IsInterface)
				throw new SystemException("Specified type is not an interface!");
			return gObj.GetInterfacesInChildren<T>().FirstOrDefault();
		}
		
		/// <summary>
		/// Gets all monobehaviours in children that implement the interface of type T (casted to T)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="gObj"></param>
		/// <returns></returns>
		public static T[] GetInterfacesInChildren<T>(this GameObject gObj)
		{
			if (!typeof(T).IsInterface)
				throw new SystemException("Specified type is not an interface!");
			
			var mObjs = gObj.GetComponentsInChildren<MonoBehaviour>();
			
			List<T> interfaces = new List<T>();
			foreach (object a in mObjs)
			{
				if (a != null)
				{
					if (a.GetType().GetInterfaces().Any(k => k == typeof(T)))
					{
						interfaces.Add((T)a);
					}
				}
			}
			return interfaces.ToArray();
			//return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
		}
	}
}

