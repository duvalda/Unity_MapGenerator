using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sorting{
	static public List<Vector2> InsertionSorting(List<Vector2> sites)
	{
		List<Vector2> orderedSites = new List<Vector2>();
		uint count = 0;

		foreach (Vector2 point in sites)
		{
			bool added = false;

			for (int i = 0; i < count; i++)
			{
				if ( point.x <= orderedSites[i].x)
				{
					orderedSites.Insert(i, point);
					added = true;
					count++;
					break;
				}
			}

			if (!added)
			{
				orderedSites.Add(point);
				count++;
			}
		}

		return orderedSites;
	}

	static int QuickSort_Partition(List<Vector2> list, int low, int high)
	{
		Vector2 pivot = list[high];
		Vector2 tmp;
		int i = low;
		for (int j = low ; j < high ; j++)
		{
			// if j is inferior to the pivot
			if ( list[j].x <= pivot.x )
			{
				// swap j with i
				tmp = list[i];
				list[i] = list[j];
				list[j] = tmp;
				i++;
			}
		}
		// swap i with highest bound : put the pivot to the end
		tmp = list[i];
		list[i] = list[high];
		list[high] = tmp;
		// return new pivot
		return i;
	}

	static public void QuickSort(List<Vector2> list, int low, int high)
	{
		if ( low < high )
		{
			int pivot = QuickSort_Partition(list, low, high);
			QuickSort(list, low, pivot - 1);
			QuickSort(list, pivot + 1, high);
		}
	}
}
