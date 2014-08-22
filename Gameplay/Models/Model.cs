using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Gameplay.Models
{
	public class Model
	{
		static private int idCounter = 0;
		private int _id;
		
		public Model()
		{
			_id = ++idCounter;
		}
		
		public virtual void Destroy()
		{
			
		}
		
		public virtual void Reset()
		{
			
		}

		public virtual void Update(float deltaTime)
		{
			
		}
		
		public int ID
		{
			get { return _id; }
		}
	}
}
