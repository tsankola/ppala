using UnityEngine;
using System.Collections;

public class Coordinate/* : MonoBehaviour*/ {
	
	public readonly int x, y, z;
	
	private Coordinate(){
	}
	
	public Coordinate(int x, int y, int z){
		this.x = x; this.y = y; this.z = z;
		invariant();
	}

	public Coordinate(Coordinate otherCoordinate) {
		this.x = otherCoordinate.x; this.y = otherCoordinate.y; this.z = otherCoordinate.z;
	}

	public int distanceTo(Coordinate other)
	{
		return ((Mathf.Abs (this.x - other.x) + Mathf.Abs (this.y - other.y) + Mathf.Abs (this.z - other.z)) / 2);
	}


	// Addition
	public static Coordinate operator +(Coordinate c1, Coordinate c2)
	{
		return new Coordinate (c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
	}

	public static Coordinate operator +(int[] c1, Coordinate c2)
	{
		if (c1.Length != 3)
			throw new System.ArgumentException();
		return new Coordinate (c1[0] + c2.x, c1[1] + c2.y, c1[2] + c2.z);
	}

	public static Coordinate operator +(Coordinate c1, int[] c2)
	{
		return c2 + c1;
	}

	// Substraction
	public static Coordinate operator -(Coordinate c1, Coordinate c2)
	{
		return new Coordinate (c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
	}
	
	public static Coordinate operator -(int[] c1, Coordinate c2)
	{
		if (c1.Length != 3)
			throw new System.ArgumentException();
		return new Coordinate (c1[0] - c2.x, c1[1] - c2.y, c1[2] - c2.z);
	}
	
	public static Coordinate operator -(Coordinate c1, int[] c2)
	{
		return c2 - c1;
	}

	// EQ
	public static bool operator ==(Coordinate c1, Coordinate c2)
	{
		return c1.x == c2.x && c1.y == c2.y && c1.z == c2.z;
	}

	public static bool operator ==(int[] c1, Coordinate c2)
	{
		return c1[0] == c2.x && c1[1] == c2.y && c1[2] == c2.z;
	}

	public static bool operator ==(Coordinate c1, int[] c2)
	{
		return c2 == c1;
	}

	// NEQ
	public static bool operator !=(Coordinate c1, Coordinate c2)
	{
		return !(c1 == c2); 
	}
	
	public static bool operator !=(int[] c1, Coordinate c2)
	{
		return !(c1 == c2); 
	}

	public static bool operator !=(Coordinate c1, int[] c2)
	{
		return !(c1 == c2); 
	}


	// Typecast
	public static explicit operator Coordinate(int[] a)
	{
		if (a.Length != 3)
			throw new System.InvalidCastException ();
		return new Coordinate (a [0], a [1], a [2]);
	}
	
	public override string ToString()
	{
		return "x:" + x + " y:" + y + " z:" + z;
	}

	public bool Equals(Coordinate other) {
		return this == other;
	}

	public override bool Equals(object other) {
		try {
			return (Coordinate)other == this;
		} catch (System.InvalidCastException ice) {
			return base.Equals(other);
		}
	}

	public override int GetHashCode() {
		int hash = 13;
		hash = (hash * 7) + x.GetHashCode();
		hash = (hash * 7) + y.GetHashCode();
		hash = (hash * 7) + z.GetHashCode();
		return hash;
	}

	private bool invariant(){	// Exception or bool? Oh well...
		if (x + y + z != 0)
			throw new System.ArithmeticException(); // return false;
		return true;
	}
}