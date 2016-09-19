using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

	private List<Vector2> m_points;
	public uint m_pointsNumber = 100;
	public float m_height = 50;
	public float m_width = 100;

	// Use this for initialization
	void Start () {
		Generate();
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if ( Input.anyKeyDown)
			Generate();
		*/
	}

	void Generate ()
	{
		m_points = new List<Vector2> ();
		// Generate points at random
		for ( int i = 0 ; i < m_pointsNumber ; i++ )
			m_points.Add( new Vector2(
				UnityEngine.Random.Range (0, m_width),
				UnityEngine.Random.Range (0, m_height))
			);
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.red;

		if (m_points != null)
		{
			foreach( Vector2 point in m_points)
				Gizmos.DrawSphere(point , 0.2f);
		}
	}

	public void PrintPoints(List<Vector2> points)
	{
		Debug.Log("------------------  PrintPoints  --------------------------");
		uint count = 0;
		foreach( Vector2 point in points)
		{
			Debug.Log(count + " : (" + point.x + "," + point.y + ")");
			count++;
		}
	}
}
