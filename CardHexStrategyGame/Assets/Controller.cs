using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controller : MonoBehaviour {

	// Use this for initialization
	void Start () {

		// debug code, quick and dirty.
		Debug.Log ("aaargh");
		HexGrid hg = new HexGrid (3);
		Hashtable grid = hg.getGrid ();
		foreach (Hex h in grid.Values) 
		{
			Debug.Log (h.ToString ());
		}
		Debug.Log(new Coordinate(0,0,0) + new int[]{0,1,-1});
		Debug.Log(new int[]{0,1,-1} + new Coordinate(0,0,0));
		Debug.Log(new Coordinate(0,0,0) == new Coordinate(0,0,0));
		Debug.Log(new Coordinate(0,0,0).Equals(new Coordinate(0,0,0)));

		if (grid.ContainsKey(new Coordinate(0,0,0)))
          Debug.Log("Jep");
		Debug.Log("neighbours:");
		Hashtable neighbours = hg.getNeighbours(hg.getHex(new Coordinate(0,0,0)));
		foreach (Hex h in neighbours.Values)
		{
			Debug.Log (h.ToString ());
		}
		Hex h1 = hg.getHex(new Coordinate(-3,0,3));
		Hex h2 = hg.getHex(new Coordinate(0,3,-3));
		Hex h3 = hg.getHex(new Coordinate(-3,1,2));
		Debug.Log ("Distance: "+ hg.distance(h3,h2));
		SortedList<int, Hex> route = hg.getRoute(hg.getHex(h1.coordinates), hg.getHex (h2.coordinates));
		foreach(KeyValuePair <int, Hex> kvp in route) {
			Debug.Log(kvp.Key + " " + kvp.Value);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
