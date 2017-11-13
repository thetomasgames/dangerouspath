using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuple<T1,T2>
{
	public T1 first { get; private set; }

	public T2 second{ get; private set; }

	public Tuple (T1 first, T2 second)
	{
		this.first = first;
		this.second = second;
	}

	public override int GetHashCode ()
	{
		return first.GetHashCode () + second.GetHashCode () * 1000;
	}

	public override bool Equals (object obj)
	{
		return obj != null && obj.GetHashCode () == this.GetHashCode ();
	}

	public override string ToString ()
	{
		return string.Format ("[Tuple: first={0}, second={1}]", first, second);
	}
}
