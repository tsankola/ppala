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
	public List<KeyValuePair<int,Hex>>  getRoute(Hex start, Hex goal) {
		if (!(grid.Contains(start.coordinates) && grid.Contains(goal.coordinates)))
			throw new System.ArgumentException("Unknown hexes");
		List<routeNode> route = AStar(start, goal);
		List<KeyValuePair<int, Hex>> retList = new List<KeyValuePair<int, Hex>>();
		route.ForEach(delegate(routeNode rn) {
			retList.Add(new KeyValuePair<int, Hex>(rn.cost, rn.hex));
		});
		return retList;
	}

	private class routeNode {
		public int cost { get; set; }
		public int estimate { get; set; }
		public Hex hex { get; set; }
		public routeNode(int cost, int estimate, Hex hex) {
			this.cost = cost; this.estimate = estimate; this.hex = hex;
		}
	}

	// Implementation of the route search algorithm
	private List<routeNode> AStar(Hex start, Hex goal)
	{
		List<routeNode> closedList = new List<routeNode>();
		List<routeNode> openList = new List<routeNode>();
		List<routeNode> finalPath = new List<routeNode>();
		Dictionary<routeNode, routeNode> pathTree = new Dictionary<routeNode, routeNode>();
		routeNode goalNode = null;
		openList.Add(new routeNode(0, 0, start));
		pathTree.Add(openList[0], null);

		if (start == goal) {
			closedList.Add(openList[0]);
			goalNode = openList[0];
			openList.Clear();
		}


		while (openList.Count != 0) {
			openList.Sort (delegate(routeNode rn1, routeNode rn2) {
//				return rn1.estimate < rn2.estimate ? -1 : (rn1.estimate == rn2.estimate ? (distance(rn1.hex, goal) < distance(rn2.hex, goal) ? -1 : (distance(rn1.hex, goal) == distance(rn2.hex, goal) ? 0 : 1)) : 1);
				return rn1.estimate < rn2.estimate ? -1 : (rn1.estimate == rn2.estimate ? 0 : 1);
			});

			routeNode parentNode = openList[0];
			openList.RemoveAt(0);
			Debug.Log("I am at parent " +parentNode.hex + " cost_start: " + parentNode.cost + " estimate: " +parentNode.estimate);

			if (parentNode.hex == goal) {
				Debug.Log("Gooooooooooooooooooooooooooooooooooooooooooooooooooal " + parentNode.hex);
				goalNode = parentNode;
				closedList.Add(parentNode);
				openList.Clear();
				break;
			}

			Hashtable children = getNeighbours(parentNode.hex);

			foreach (Hex child in children.Values) {
				bool already_visited = false;
				closedList.ForEach(delegate(routeNode rn){
					if (rn.hex == child)
						already_visited = true;
				});
				if (already_visited)
					continue;

				int child_cost = parentNode.cost + cost(parentNode.hex,child);
				int child_estimate = distance(child, goal);
				int total_estimate = child_cost + child_estimate;
				Debug.Log("I am at child " +child.coordinates  + " cost_start: " +child_cost + " estimate: "+child_estimate);
				bool already_pending_with_better_estimate = false;

				openList.ForEach(delegate(routeNode rn){
					if (rn.hex == child && rn.estimate < total_estimate)
						already_pending_with_better_estimate = true;
				});
				if (already_pending_with_better_estimate)
					continue;

				routeNode childNode = new routeNode(child_cost, total_estimate, child);
				openList.Add(childNode);
				pathTree.Add(childNode, parentNode);
			}
			closedList.Add(parentNode);
		}

		while (goalNode != null) {
			finalPath.Add(goalNode);
			goalNode = pathTree[goalNode];
		}
		return finalPath;
	}
}
