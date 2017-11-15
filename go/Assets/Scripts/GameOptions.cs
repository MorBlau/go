using UnityEngine;
using System.Collections;

public static class GameOptions {

	public static readonly bool debug = false;


	public static readonly int NO_PLAYER_CHECKED = -2;
	public static readonly int NO_PLAYER = -1;
	public static readonly int WHITE = 0;
	public static readonly int BLACK = 1;

	private static int gridCount = 19;
	public static int player1 = GameOptions.WHITE;
	public static int player2 = GameOptions.BLACK;

	public static string player1Name = "Alice";
	public static string player2Name = "Bob";

	public static void SetGridCount(int count) {
		gridCount = count;
	}

	public static int GetGridCount() {
		return gridCount;
	}
		
}
