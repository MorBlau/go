using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class GameScore {

	public static Dictionary<int, int> count;

	public static Dictionary<int, int> finalStoneCount;

	public static int[,] playerControlMap;

	public static bool[,] chainMap;

	private static GameObject eventSystem;
	private static Text player1ScoreText;
	private static Text player2ScoreText;
	private static PanelController panelController;

	public static void Init() {
		ResetGame ();
//		capturedSpaceCount = new Dictionary<int, int> ();
//		eventSystem = BoardController.getEventSystem ();
		player1ScoreText = BoardController.GetPlayer1ScoreText ();
		player2ScoreText = BoardController.GetPlayer2ScoreText ();
		panelController = GameObject.FindGameObjectWithTag ("Panel").GetComponent<PanelController>();
	}

	public static void ResetCurrentCount() {
		count = new Dictionary<int, int> ();
		count.Add(GameOptions.player1, 0);
		count.Add(GameOptions.player2, 0);
	}

	public static void ResetFinalStoneCount() {
		finalStoneCount = new Dictionary<int, int> ();
		finalStoneCount.Add(GameOptions.player1, 0);
		finalStoneCount.Add(GameOptions.player2, 0);
	}

	public static void SetFinalStoneCount() {
		finalStoneCount = count;
		ResetCurrentCount ();
	}

	public static void ResetGame() {
		ResetCurrentCount ();
		ResetFinalStoneCount ();
//		capturedSpaceCount.Add(GameOptions.player1, 0);
//		capturedSpaceCount.Add(GameOptions.player2, 0);
	}

	public static void AddScore(int scoreToAdd, int player) {
		int currentScore = 0;
		bool valueExists = GameScore.count.TryGetValue (player, out currentScore);
		if (valueExists) {
			GameScore.count.Remove (player);
			GameScore.count.Add (player, currentScore + scoreToAdd);
		} else {
			GameScore.count.Add (player, scoreToAdd);
		}

		// also update HUD
		panelController.SetNewScore (player);

//		if (player == GameOptions.player1) {
//			player1ScoreText.text = string.Format("{0}", currentScore + scoreToAdd);
//		} else {
//			player2ScoreText.text = string.Format("{0}", currentScore + scoreToAdd);
//		}

	}
}

