using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Math
{
	public class Point
	{
		private int _x;
		private int _y;

		public Point()
		{
			_x = 0;
			_y = 0;
		}

		public Point(int x, int y)
		{
			_x = x;
			_y = y;
		}

		public int x
		{
			get { return _x; }
			set { _x = value; }
		}

		public int y
		{
			get { return _y; }
			set { _y = value; }
		}
	}
}
