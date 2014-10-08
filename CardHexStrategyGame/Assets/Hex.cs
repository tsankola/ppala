using UnityEngine;
using System.Collections;

public class Hex {

	readonly public Coordinate coordinates;

	private Hex()
	{
	}

	public Hex(Coordinate coordinates)
	{
		this.coordinates = coordinates;
	}

	public int distanceTo(Hex other)
	{
		return this.coordinates.distanceTo (other.coordinates);
	}

	public override string ToString()
	{
		return "Hex: " + coordinates.ToString ();
	}

}
