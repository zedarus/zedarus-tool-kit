using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit
{
	/// <summary>
	/// Easing functions.
	/// t = time, 
	/// b = startvalue, 
	/// c = change in value, 
	/// d = duration
	/// 
	/// source: https://github.com/jesusgollonet/processing-penner-easing/tree/master/src
	/// </summary>
	public struct Easing
	{
		public struct Linear
		{
			public static float easeNone(float t, float b, float c, float d)
			{
				return c * t / d + b;
			}

			public static float easeIn(float t, float b, float c, float d)
			{
				return c * t / d + b;
			}

			public static float easeOut(float t, float b, float c, float d)
			{
				return c * t / d + b;
			}

			public static float easeInOut(float t, float b, float c, float d)
			{
				return c * t / d + b;
			}
		}

		public struct Back
		{
			public static float  easeIn(float t, float b, float c, float d)
			{
				float s = 1.70158f;
				return c * (t /= d) * t * ((s + 1) * t - s) + b;
			}

			public static float  easeIn(float t, float b, float c, float d, float s)
			{
				return c * (t /= d) * t * ((s + 1) * t - s) + b;
			}

			public static float  easeOut(float t, float b, float c, float d)
			{
				float s = 1.70158f;
				return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
			}

			public static float  easeOut(float t, float b, float c, float d, float s)
			{
				return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
			}

			public static float  easeInOut(float t, float b, float c, float d)
			{
				float s = 1.70158f;
				if ((t /= d / 2) < 1)
					return c / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + b;
				return c / 2 * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + b;
			}

			public static float  easeInOut(float t, float b, float c, float d, float s)
			{	
				if ((t /= d / 2) < 1)
					return c / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + b;
				return c / 2 * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + b;
			}
		}

		public struct Bounce
		{
			public static float  easeIn(float t, float b, float c, float d)
			{
				return c - easeOut(d - t, 0, c, d) + b;
			}

			public static float  easeOut(float t, float b, float c, float d)
			{
				if ((t /= d) < (1 / 2.75f))
				{
					return c * (7.5625f * t * t) + b;
				}
				else if (t < (2 / 2.75f))
				{
					return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f) + b;
				}
				else if (t < (2.5 / 2.75))
				{
					return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f) + b;
				}
				else
				{
					return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f) + b;
				}
			}

			public static float  easeInOut(float t, float b, float c, float d)
			{
				if (t < d / 2)
					return easeIn(t * 2, 0, c, d) * .5f + b;
				else
					return easeOut(t * 2 - d, 0, c, d) * .5f + c * .5f + b;
			}
		}

		public struct Circ
		{
			public static float  easeIn(float t, float b, float c, float d)
			{
				return -c * ((float)Mathf.Sqrt(1 - (t /= d) * t) - 1) + b;
			}

			public static float  easeOut(float t, float b, float c, float d)
			{
				return c * (float)Mathf.Sqrt(1 - (t = t / d - 1) * t) + b;
			}

			public static float  easeInOut(float t, float b, float c, float d)
			{
				if ((t /= d / 2) < 1)
					return -c / 2 * ((float)Mathf.Sqrt(1 - t * t) - 1) + b;
				return c / 2 * ((float)Mathf.Sqrt(1 - (t -= 2) * t) + 1) + b;
			}
		}

		public struct Cubic
		{
			public static float easeIn(float t, float b, float c, float d)
			{
				return c * (t /= d) * t * t + b;
			}

			public static float easeOut(float t, float b, float c, float d)
			{
				return c * ((t = t / d - 1) * t * t + 1) + b;
			}

			public static float easeInOut(float t, float b, float c, float d)
			{
				if ((t /= d / 2) < 1)
					return c / 2 * t * t * t + b;
				return c / 2 * ((t -= 2) * t * t + 2) + b;
			}
		}

		public struct Elastic
		{
			public static float  easeIn(float t, float b, float c, float d)
			{
				if (t == 0)
					return b;
				if ((t /= d) == 1)
					return b + c;  
				float p = d * .3f;
				float a = c; 
				float s = p / 4;
				return -(a * (float)Mathf.Pow(2, 10 * (t -= 1)) * (float)Mathf.Sin((t * d - s) * (2 * (float)Mathf.PI) / p)) + b;
			}

			public static float  easeIn(float t, float b, float c, float d, float a, float p)
			{
				float s;
				if (t == 0)
					return b;
				if ((t /= d) == 1)
					return b + c;  
				if (a < Mathf.Abs(c))
				{
					a = c;
					s = p / 4;
				}
				else
				{
					s = p / (2 * (float)Mathf.PI) * (float)Mathf.Asin(c / a);
				}
				return -(a * (float)Mathf.Pow(2, 10 * (t -= 1)) * (float)Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p)) + b;
			}

			public static float  easeOut(float t, float b, float c, float d)
			{
				if (t == 0)
					return b;
				if ((t /= d) == 1)
					return b + c;  
				float p = d * .3f;
				float a = c; 
				float s = p / 4;
				return (a * (float)Mathf.Pow(2, -10 * t) * (float)Mathf.Sin((t * d - s) * (2 * (float)Mathf.PI) / p) + c + b);	
			}

			public static float  easeOut(float t, float b, float c, float d, float a, float p)
			{
				float s;
				if (t == 0)
					return b;
				if ((t /= d) == 1)
					return b + c;  
				if (a < Mathf.Abs(c))
				{
					a = c;
					s = p / 4;
				}
				else
				{
					s = p / (2 * (float)Mathf.PI) * (float)Mathf.Asin(c / a);
				}
				return (a * (float)Mathf.Pow(2, -10 * t) * (float)Mathf.Sin((t * d - s) * (2 * (float)Mathf.PI) / p) + c + b);	
			}

			public static float  easeInOut(float t, float b, float c, float d)
			{
				if (t == 0)
					return b;
				if ((t /= d / 2) == 2)
					return b + c; 
				float p = d * (.3f * 1.5f);
				float a = c; 
				float s = p / 4;
				if (t < 1)
					return -.5f * (a * (float)Mathf.Pow(2, 10 * (t -= 1)) * (float)Mathf.Sin((t * d - s) * (2 * (float)Mathf.PI) / p)) + b;
				return a * (float)Mathf.Pow(2, -10 * (t -= 1)) * (float)Mathf.Sin((t * d - s) * (2 * (float)Mathf.PI) / p) * .5f + c + b;
			}

			public static float  easeInOut(float t, float b, float c, float d, float a, float p)
			{
				float s;
				if (t == 0)
					return b;
				if ((t /= d / 2) == 2)
					return b + c;  
				if (a < Mathf.Abs(c))
				{
					a = c;
					s = p / 4;
				}
				else
				{
					s = p / (2 * (float)Mathf.PI) * (float)Mathf.Asin(c / a);
				}
				if (t < 1)
					return -.5f * (a * (float)Mathf.Pow(2, 10 * (t -= 1)) * (float)Mathf.Sin((t * d - s) * (2 * (float)Mathf.PI) / p)) + b;
				return a * (float)Mathf.Pow(2, -10 * (t -= 1)) * (float)Mathf.Sin((t * d - s) * (2 * (float)Mathf.PI) / p) * .5f + c + b;
			}
		}

		public struct Expo
		{
			public static float  easeIn(float t, float b, float c, float d)
			{
				return (t == 0) ? b : c * (float)Mathf.Pow(2, 10 * (t / d - 1)) + b;
			}

			public static float  easeOut(float t, float b, float c, float d)
			{
				return (t == d) ? b + c : c * (-(float)Mathf.Pow(2, -10 * t / d) + 1) + b;	
			}

			public static float  easeInOut(float t, float b, float c, float d)
			{
				if (t == 0)
					return b;
				if (t == d)
					return b + c;
				if ((t /= d / 2) < 1)
					return c / 2 * (float)Mathf.Pow(2, 10 * (t - 1)) + b;
				return c / 2 * (-(float)Mathf.Pow(2, -10 * --t) + 2) + b;
			}
		}

		public struct Quad
		{
			public static float  easeIn(float t, float b, float c, float d)
			{
				return c * (t /= d) * t + b;
			}

			public static float  easeOut(float t, float b, float c, float d)
			{
				return -c * (t /= d) * (t - 2) + b;
			}

			public static float  easeInOut(float t, float b, float c, float d)
			{
				if ((t /= d / 2) < 1)
					return c / 2 * t * t + b;
				return -c / 2 * ((--t) * (t - 2) - 1) + b;
			}
		}

		public struct Quart
		{
			public static float  easeIn(float t, float b, float c, float d)
			{
				return c * (t /= d) * t * t * t + b;
			}

			public static float  easeOut(float t, float b, float c, float d)
			{
				return -c * ((t = t / d - 1) * t * t * t - 1) + b;
			}

			public static float  easeInOut(float t, float b, float c, float d)
			{
				if ((t /= d / 2) < 1)
					return c / 2 * t * t * t * t + b;
				return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
			}
		}

		public struct Quint
		{
			public static float easeIn(float t, float b, float c, float d)
			{
				return c * (t /= d) * t * t * t * t + b;
			}

			public static float easeOut(float t, float b, float c, float d)
			{
				return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
			}

			public static float easeInOut(float t, float b, float c, float d)
			{
				if ((t /= d / 2) < 1)
					return c / 2 * t * t * t * t * t + b;
				return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
			}
		}

		public struct Sine
		{
			public static float  easeIn(float t, float b, float c, float d)
			{
				return -c * (float)Mathf.Cos(t / d * (Mathf.PI / 2)) + c + b;
			}

			public static float  easeOut(float t, float b, float c, float d)
			{
				return c * (float)Mathf.Sin(t / d * (Mathf.PI / 2)) + b;	
			}

			public static float  easeInOut(float t, float b, float c, float d)
			{
				return -c / 2 * ((float)Mathf.Cos(Mathf.PI * t / d) - 1) + b;
			}
		}
	}
}
