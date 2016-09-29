using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct LineSegment{
	public Vector2 right;
	public Vector2 left;
}

public class Parabola{
	public Vector2 point;
	public Parabola previous = null;
	public Parabola next = null;

	public Parabola(Vector2 p) {point = p;}
	public Parabola(Parabola para) {
		point = para.point;
		previous = para.previous;
		next = para.next;
	}
}

public class Event{
	public Vector2 point;
	public Vector2 breakPoint;

	public Event(Vector2 p, Vector2 bp) {point = p ; breakPoint = bp;}
}

public class Voronoi{

	private List<Vector2> m_sites;
	private List<LineSegment> m_edges;
	private List<Event> m_events;
	// First parabola of the beachline
	private Parabola m_root;

	// We assume that we are in a rectangle and the sweepline goes from left to right
	Rect m_area;
	float m_sweepLineX;

	public Voronoi(Rect area, List<Vector2> sites)
	{
		m_area = area;
		m_sites = sites;
		Sorting.QuickSort(m_sites, 0, m_sites.Count - 1);

		m_sweepLineX = m_area.left;
	}

	private Event NextEvent()
	{
		return new Event(Vector2.zero, Vector2.zero);
	}

	public List<LineSegment> Diagram()
	{
		while (m_sites.Count != 0)
		{
			// Process circle events prior to sites event
			if (m_events.Count != 0 && m_events[0].point.x <= m_sites[0].x)
				ProcessEvent();
			else
				ProcessSite();
		}
		return m_edges;
	}

	private void ProcessEvent()
	{

	}

	private void ProcessSite()
	{
		Vector2 site = m_sites[0];
		m_sweepLineX = site.x;
		m_sites.RemoveAt(0);
		AddParabola(site);
	}

	private void AddParabola(Vector2 point)
	{
		if (m_root != null)
		{
			m_root = new Parabola(point);
			return;
		}

		Parabola para = new Parabola(point);
		Parabola i;

		// Look for intersections with the beach line
		for (i = m_root ; i != null; i = i.next)
		{
			Vector2 intersection = ParabolaIntersection(i,para, m_sweepLineX);
			if (IsValid(intersection))
			{
				// Duplicate i and insert new parabola between i and i'
				para.next = new Parabola(i);

				para.next.previous = para;
				i.next = para;
				para.previous = i;

				return;
			}
		}

		// If no intersection in our area, find where to insert it
		for (i = m_root ; i != null ; i = i.next)
		{
			if (para.point.y < i.point.y)
			{
				para.next = i;
				i = para;
				para.next.previous = para;
				return;
			}
		}

		// Add parabola to the end : find last parabola
		for (i = m_root ; i.next != null ; i = i.next);
		i.next = para;
		para.previous = i;
	}

	/*
	* ParabolaIntersection
	* Assuming the sweepline is vertical and the parabolas are on the left of it.
	* Return the intersection between p1 and p2.
	*/
	private Vector2 ParabolaIntersection( Parabola p1, Parabola p2, float slx /* SweepLineX */)
	{
		Vector2 result = new Vector2(-1.0f,-1.0f);
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

	private void CheckCircleEvent(Parabola p)
	{
		if ( p.previous == null || p.next == null)
			return;

		// center corresponds to the new breakpoint
		Vector2 cc = CircleCenter(p.previous.point, p.point, p.next.point);
		if (IsValid(cc))
		{
			// point event represents when the sweepline meets the event.
			Vector2 point = new Vector2(cc.x + Distance(p.point, cc), cc.y);
			m_events.Add(new Event(point,cc));
		}
	}

	private float Distance(Vector2 a, Vector2 b)
	{
		return Mathf.Sqrt((a.x - b.x)*(a.x - b.x) + (a.y - b.y)*(a.y - b.y));
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

	private bool IsValid(Vector2 p)
	{
		return m_area.Contains(p);
	}
}
