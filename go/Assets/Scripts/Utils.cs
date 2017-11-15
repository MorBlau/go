
using UnityEngine;
using System.Collections;

public static class Utils 
{

	public static int[] Vector2ToIntArray(Vector2 vector2) {
		
		return new int[] { Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y) };
	}

}