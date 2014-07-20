using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Helpers
{
	public class MathHelper
	{
		static public float FindDistanceToLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
		{
			Vector2 closest;
			float dx = lineEnd.x - lineStart.x;
			float dy = lineEnd.y - lineStart.y;
			if ((dx == 0) && (dy == 0))
			{
				// It's a point not a line segment.
				closest = lineStart;
				dx = point.x - lineStart.x;
				dy = point.y - lineStart.y;
				return Mathf.Sqrt(dx * dx + dy * dy);
			}

			// Calculate the t that minimizes the distance.
			float t = ((point.x - lineStart.x) * dx + (point.y - lineStart.y) * dy) / (dx * dx + dy * dy);

			// See if this represents one of the segment's
			// end points or a point in the middle.
			if (t < 0)
			{
				closest = new Vector2(lineStart.x, lineStart.y);
				dx = point.x - lineStart.x;
				dy = point.y - lineStart.y;
			} else if (t > 1)
			{
				closest = new Vector2(lineEnd.x, lineEnd.y);
				dx = point.x - lineEnd.x;
				dy = point.y - lineEnd.y;
			} else
			{
				closest = new Vector2(lineStart.x + t * dx, lineStart.y + t * dy);
				dx = point.x - closest.x;
				dy = point.y - closest.y;
			}
		
			return Mathf.Sqrt(dx * dx + dy * dy);
		}
	
		static public Vector2 ProjectPointOnLine(Vector2 line1, Vector2 line2, Vector2 toProject)
		{
			float m = (line2.y - line1.y) / (line2.x - line1.x);
			float b = line1.y - (m * line1.x);
		
			float x = (m * toProject.y + toProject.x - m * b) / (m * m + 1);
			float y = (m * m * toProject.y + m * toProject.x + b) / (m * m + 1);
		
			return new Vector2(x, y);
		}

		static public bool FasterLineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
		{
			Vector2 a = p2 - p1;
			Vector2 b = p3 - p4;
			Vector2 c = p1 - p3;
			
			float alphaNumerator = b.y * c.x - b.x * c.y;
			float alphaDenominator = a.y * b.x - a.x * b.y;
			float betaNumerator = a.x * c.y - a.y * c.x;
			float betaDenominator = a.y * b.x - a.x * b.y;
			
			bool doIntersect = true;
			
			if (alphaDenominator == 0 || betaDenominator == 0)
			{
				doIntersect = false;
			} else
			{
				
				if (alphaDenominator > 0)
				{
					if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
					{
						doIntersect = false;
						
					}
				} else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
				{
					doIntersect = false;
				}
				
				if (betaDenominator > 0)
				{
					if (betaNumerator < 0 || betaNumerator > betaDenominator)
					{
						doIntersect = false;
					}
				} else if (betaNumerator > 0 || betaNumerator < betaDenominator)
				{
					doIntersect = false;
				}
			}
			
			return doIntersect;
		}

		static bool PointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
		{
			Vector2 v0 = b - c;
			Vector2 v1 = a - c;
			Vector2 v2 = point - c;
			float dot00 = Vector2.Dot(v0, v0);
			float dot01 = Vector2.Dot(v0, v1);
			float dot02 = Vector2.Dot(v0, v2);
			float dot11 = Vector2.Dot(v1, v1);
			float dot12 = Vector2.Dot(v1, v2);
			float invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
			float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
			float v = (dot00 * dot12 - dot01 * dot02) * invDenom;
			return (u > 0.0f) && (v > 0.0f) && (u + v < 1.0f);
		}
	}
}
