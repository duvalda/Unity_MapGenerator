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

public class Voronoi{

	//private List<Vector2> m_vertices;
	private List<Vector2> m_sites;
	private List<LineSegment> m_edges;

	// We assume that we are in a rectangle and the sweepline goes from left to right
	Rect m_area;

	public Voronoi(Rect area, List<Vector2> sites)
	{
		m_area = area;
		m_sites = sites;
		Sorting.QuickSort(m_sites, 0, m_sites.Count - 1);
	}
		
	public List<LineSegment> Diagram()
	{

		return m_edges;
	}

	private Vector2 ParabolaIntersection( Parabola a, Parabola b, double sweeplineX)
	{
		Vector2 result = Vector2.zero;

		// solve equation

		return result;
	}
}
