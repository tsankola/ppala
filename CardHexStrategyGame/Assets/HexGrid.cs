using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexGrid {	// A symmetric hex grid. TODO: Extract abstract superclass in order to implement grids of different shapes
	struct Bounds
	{
		public readonly int lower;
		public readonly int upper;

		public Bounds(int lower, int upper)
		{
			this.lower = lower;
			this.upper = upper;
		}
	};

	private readonly Bounds x_range, y_range, z_range;
	private Hashtable grid;//<Coordinate, Hex> // TODO: Convert to Dictionary

	public HexGrid(int radius)
	{
		if (radius < 1)		// Maybe it could work with negative radius as well(?)
			throw new System.ArgumentException();
		x_range = y_range = z_range = new Bounds(-radius, radius);
		grid = new Hashtable ();
		createCoordinateGrid ();
	}


	// Initializes the grid
	private void createCoordinateGrid(){
		for (int i = x_range.lower; i <= x_range.upper; i++) {
			for (int j = y_range.lower; j <= y_range.upper; j++) {
				for (int k = z_range.lower; k <= z_range.upper; k++) {
					if (i + j + k == 0){
						Coordinate coord = new Coordinate(i, j, k);
						grid.Add(coord, new Hex(coord));
					}
				}
			}
		}
	}

	// Returns the hex at given location
	public Hex getHex(Coordinate location) {
		return (Hex)grid[location];
	}

	// Returns a hashtable<Coordinate, Hex> of the neighbours for the given Hex.
	public Hashtable getNeighbours(Hex center)
	{
		Hashtable neighbourHash = new Hashtable ();
		int[][] neighbour_permutations = {new int[]{1, -1, 0}, new int[]{1, 0, -1}, new int[]{0, 1, -1}, new int[]{-1, 1, 0}, new int[]{-1, 0, 1}, new int[]{0, -1, 1}};
		for (int i = 0; i < neighbour_permutations.Length; i++)
        {
			Coordinate neighbour = center.coordinates + neighbour_permutations[i];
			Debug.Log("gn: " + neighbour);
			if (grid.Contains(neighbour))
			{
				Debug.Log("Neigh! " + grid[neighbour]);
				neighbourHash.Add(neighbour, grid[neighbour]);
			}
		}
		return neighbourHash;
	}

	public Hashtable getGrid()
	{
		return grid;
	}

	// Distance from h1 to h2
	public int distance(Hex h1, Hex h2) {
		return h1.coordinates.distanceTo(h2.coordinates);
	}

	// Cost to travel from h1 to h2. This should reflect difficult and impassable terrain
	public int cost(Hex h1, Hex h2) {
		return h1.coordinates.distanceTo(h2.coordinates);
	}

	// Returns the shortest route between start and goal hexes
	public SortedList getRoute(Hex start, Hex goal) {
		if (!(grid.Contains(start.coordinates) && grid.Contains(goal.coordinates)))
			throw new System.ArgumentException("Unknown hexes");
		return AStar(start, goal);
	}

	// Implementation of the route search algorithm
	private SortedList AStar(Hex start, Hex goal)
	{
		SortedList closedSet = new SortedList();	// The finished route, basically
		SortedList openset = new SortedList();
		openset.Add(0, start);
		if (start == goal) {
			closedSet.Add(0, start);
			openset.Clear();
		}

		while (openset.Count != 0) {
			Hex parent = (Hex)openset.GetByIndex(0);
			int parent_cost = (int)openset.GetKey (0);
			openset.RemoveAt(0);
			Hashtable children = getNeighbours(start);
			foreach (Hex child in children.Values) {
				if (child == goal) {
					closedSet.Add(parent_cost + cost(parent,child), child);
					openset.Clear();
					break;
				}
				int cost_to_start = parent_cost + cost(parent,child);
				int distance_to_goal = distance(child, goal);
				int estimate = cost_to_start + distance_to_goal;
				if (openset.ContainsValue(child))
					if ((int)openset.GetKey(openset.IndexOfValue(child)) <= estimate)
						continue;
				if (closedSet.ContainsValue(child))
					if ((int)closedSet.GetKey(openset.IndexOfValue(child)) <= estimate)
						continue;
				openset.Add(estimate, child);
			}
			if (closedSet.ContainsValue(parent)) {	// Not sure if this can happen, but just in case the parent is already in closedset and has a bigger cost than current
				if ((int)closedSet.GetKey(closedSet.IndexOfValue(parent)) > parent_cost)
					throw new System.Exception("parent cost wrong!"); // Shouldn't get here, ever.
				closedSet.RemoveAt(closedSet.IndexOfValue(parent));
			}
			closedSet.Add(parent_cost,parent);
		}

		return closedSet;
	}
}
