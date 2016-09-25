using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct LineSegment{
	public Vector2 right;
	public Vector2 left;
}

public class Parabola{
	public Vector2 point;
	public Parabola left = null;
	public Parabola right = null;

	public Parabola(Vector2 p) {point = p;}
}

public class Event{
	public enum Type{
		Site, // New point encountered
		Circle, // 3 sites on the same circle : end on an edge
		End // last event
	}

	public Vector2 point;
	public Type type;

	public Event(Vector2 p, Type t) {point = p; type = t;}
}

public class Voronoi{

	private List<Vector2> m_beachLineSites;
	private List<Vector2> m_sites;
	private List<LineSegment> m_edges;

	// We assume that we are in a rectangle and the sweepline goes from left to right
	Rect m_area;
	float m_sweepLineX;

	public Voronoi(Rect area, List<Vector2> sites)
	{
		m_area = area;
		m_sites = sites;
		Sorting.QuickSort(m_sites, 0, m_sites.Count - 1);

		m_sweepLineX = m_area.left;
		m_beachLineSites = new List<Vector2>();
	}

	private Event NextEvent()
	{
		return new Event(Vector2.zero, Event.Type.End);
	}

	public List<LineSegment> Diagram()
	{
		bool done = false;
		while(!done)
		{
			// Seek next event
			Event e = NextEvent();
			switch(e.type)
			{
			case Event.Type.Site:
				// Site event (new point) : add a Parabola for the point
				break;
			case Event.Type.Circle:
				// Circle event (3 sites on the same circle) : Remove Parabola for the first point encountered
				break;
			case Event.Type.End:
				done = true;
				break;
			}
		}
		return m_edges;
	}

	/*
	* ParabolaIntersection
	* Assuming the sweepline is vertical and the parabolas are on the left of it.
	* Return the intersection between p1 and p2.
	*/
	private Vector2 ParabolaIntersection( Parabola p1, Parabola p2, float slx /* SweepLineX */)
	{
		Vector2 result = Vector2.zero;
		// Parabola equation for parabola 1:
		// X = ((Y1² - Y²) + X1² - slx²) / 2( X1 - slx)

		float x1 = p1.point.x;
		float x2 = p2.point.x;
		float y1 = p1.point.y;
		float y2 = p2.point.y;
		Vector2 p = p1.point;

		// specific and zero-divider cases
		if (x1 == x2)
		{
			result.y = (y1 + y2) / 2.0f;
		}
		else if (x1 == slx)
		{
			result.y = y1;
			p = p2.point;
		}
		else if (x2 == slx)
		{
			result.y = y2;
		}
		else
		{
			// solve 2nd degree equation
			float z1 = 2*(x1 - slx);
			float z2 = 2*(x2 - slx);

			float a = 1/z1 - 1/z2;
			float b = 2*(x2/z2 - x1/z1);
			float c = (x1*x1 + y1*y1 - slx*slx)/z1 - (x2*x2 + y2*y2 - slx*slx)/z2;

			result.y = (-b - Mathf.Sqrt(b*b - 4*a*c))/(2*a);
			// 2nd solution
			//result.y = (-b + Mathf.Sqrt(b*b - 4*a*c))/(2*a);
		}
		result.x = ((p.y*p.y - result.y*result.y) + p.x*p.x - slx*slx) / (2*(p.x - slx));
		return result;
	}

	private Vector2 CircleCenter(Vector2 a, Vector2 b, Vector2 c)
	{
		Vector2 result = new Vector2(-1.0f,-1.0f);

		float Xba = b.x - a.x;
		float Xca = c.x - a.x;
		float Yba = b.y - a.y;
		float Yca = c.y - a.y;

		// check division by zero
		float d = 2*(Yba*Xca - Yca*Xba);
		if ( d == 0 )
			return result;

		float Yca2 = c.y*c.y - a.y*a.y;
		float Xca2 = c.x*c.x - a.x*a.x;
		float Yba2 = b.y*b.y - a.y*a.y;
		float Xba2 = b.x*b.x - a.x*a.x;

		float n = Xba*(Yca2+Xca2) - Xca*(Yba2+Xba2);

		result.y = n/d;
		result.x = (Yba2 + 2*result.y*Yba + Xba2) / (2*Xba);
		return result;
	}
}
