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
//			Debug.Log("gn: " + neighbour);
			if (grid.Contains(neighbour))
			{
//				Debug.Log("Neigh! " + grid[neighbour]);
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
	public SortedList<int,Hex>  getRoute(Hex start, Hex goal) {
		if (!(grid.Contains(start.coordinates) && grid.Contains(goal.coordinates)))
			throw new System.ArgumentException("Unknown hexes");
		List<routeNode> route = AStar(start, goal);
		SortedList<int, Hex> retList = new SortedList<int, Hex>();
		route.ForEach(delegate(routeNode rn) {
			retList.Add(rn.cost, rn.hex);
		});
		return retList;
	}

	struct routeNode {
		public int cost { get; set; }
		public int estimate { get; set; }
		public Hex hex { get; set; }
		public routeNode(int cost, int estimate, Hex hex) : this() {
			this.cost = cost; this.estimate = estimate; this.hex = hex;
		}
	}

	// Implementation of the route search algorithm
	private List<routeNode> AStar(Hex start, Hex goal)
	{
		List<routeNode> closedSet = new List<routeNode>();	// The finished route, basically
		List<routeNode> openSet = new List<routeNode>();	// The finished route, basically
		List<routeNode> route = new List<routeNode>();
		openSet.Add(new routeNode(0, 0, start));

		if (start == goal) {
			closedSet.Add(new routeNode(0,0, start));
			openSet.Clear();
		}


		while (openSet.Count != 0) {
			openSet.Sort (delegate(routeNode rn1, routeNode rn2) {
				return rn1.estimate < rn2.estimate ? -1 : (rn1.estimate == rn2.estimate ? (distance(rn1.hex, goal) < distance(rn2.hex, goal) ? -1 : (distance(rn1.hex, goal) == distance(rn2.hex, goal) ? 0 : 1)) : 1);
			});
			routeNode parentNode = openSet[0];
			openSet.RemoveAt(0);
			Hashtable children = getNeighbours(parentNode.hex);

			Debug.Log("I am at parent " +parentNode.hex + " cost_start: " + parentNode.cost + " estimate: " +parentNode.estimate);

			foreach (Hex child in children.Values) {
				if (child == goal) {
					routeNode childNode = new routeNode(parentNode.cost + cost(parentNode.hex, child), distance(child,child), child);
					route.Add(childNode);
//					closedSet.Add(new routeNode(parentNode.cost + cost(parentNode.hex, child), distance(child,child), child));
					openSet.Clear();
					break;
				}
				int child_cost = parentNode.cost + cost(parentNode.hex,child);
				int child_estimate = distance(child, goal);
				int total_estimate = child_cost + child_estimate;
				Debug.Log("I am at child " +child.coordinates  + " cost_start: " +child_cost + " estimate: "+child_estimate);
				bool already_visited_and_found_better_route = false;
				openSet.ForEach(delegate(routeNode rn){
					if (rn.hex == child && rn.estimate < total_estimate)
						already_visited_and_found_better_route = true;
				});
				if (already_visited_and_found_better_route)
					continue;
				closedSet.ForEach(delegate(routeNode rn){
					if (rn.hex == child && rn.estimate < total_estimate)
						already_visited_and_found_better_route = true;
				});
				if (!already_visited_and_found_better_route)
					openSet.Add(new routeNode(child_cost, total_estimate, child));
			}
			closedSet.ForEach(delegate(routeNode rn){ // This needs rethinking
				if (rn.hex == parentNode.hex) {
					if (rn.estimate > parentNode.estimate)
						throw new System.Exception("parent cost wrong!");
					closedSet.Remove(rn);
				}
			});
			closedSet.Add(parentNode);
		}
		return closedSet;
	}
}
