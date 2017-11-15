using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rules : MonoBehaviour {

	public static bool processingMove;

	private static int gridCount = GameOptions.GetGridCount();
	private static Vector2[] blockedCells;
	private static int currentPlayer;
	private static Rules _anInstance;
	private static LinkedList<CellData> markedCells;
	private static int markedCellCount;
	private static PanelController panelController;
	private static bool lastMoveWasPassed = false;

	public static void Init() {
		currentPlayer = GameOptions.player1;
		InitMaps ();
		processingMove = false;
		markedCells = new LinkedList<CellData>();
		panelController = GameObject.FindGameObjectWithTag ("Panel").GetComponent<PanelController> ();
		SetCorrectPassButton ();
	}

	private static void InitMaps () {
		GameScore.playerControlMap = new int[gridCount, gridCount];
		GameScore.chainMap = new bool[gridCount, gridCount];
		for (int x = 0; x < gridCount; x++) {
			for (int y = 0; y < gridCount; y++) {
				GameScore.playerControlMap [x, y] = GameOptions.NO_PLAYER;
//				GameScore.emptyAdjacentMap [x, y] = true;
			}
		}
	}

	public static void ProcessMove(CellData cellData) { //TODO: move cellData creation here

		int x = cellData.GetX();
		int y = cellData.GetY();
		int opponent = GetOpponent();

		// add player to control map
		GameScore.playerControlMap [x, y] = currentPlayer;

		// initialize marked cell list
		// check to see if there's a blocked chain
		Vector2[] cellArray = new Vector2[] {
			new Vector2(x - 1, y),new Vector2(x, y -1),new Vector2(x + 1, y),new Vector2(x, y + 1)};
		
		CaptureArray (cellArray, opponent, currentPlayer);
		Capture (cellData.GetV2(), currentPlayer, opponent);
		// ProcessCapturedSpaceScore(); // bonus

		SetNextPlayer();
		PrintMap ("playerControlMap1", GameScore.playerControlMap);
		lastMoveWasPassed = false;
		processingMove = false;


		// debug - color cells by player control
		if (GameOptions.debug) {
			for (int i = 0; i < GameScore.playerControlMap.GetLength (0); i++) {
				for (int j = 0; j < GameScore.playerControlMap.GetLength (1); j++) {
					GameObject cell;
					Dictionary <Vector2, GameObject> gridMap = BoardController.getGridMap ();
					gridMap.TryGetValue (new Vector2 (i, j), out cell);
					cell.GetComponent<CellData> ().colorCell (GameScore.playerControlMap [i, j]);
				}
			}
		}

		// debug - print score
		int player1Score = 0;
		int player2Score = 0;

		GameScore.count.TryGetValue (GameOptions.player1, out player1Score);
		GameScore.count.TryGetValue (GameOptions.player2, out player2Score);

		Debug.Log ("player1 score is: " + player1Score + ", player2 score is: " + player2Score);
	}

	private static void PrintMap(string mapName, bool[,] map) {
		int length1 = map.GetLength (0);
		int length2 = map.GetLength (1);
		int[,] intMap = new int[length1, length2];
		for (int i = 0 ; i < length1 ; i++) {
			for (int j = 0; j < length2; j++) {
				intMap [i, j] = (map[i, j] ? 1 : 0);
			}
		}
		PrintMap (mapName, intMap);
	}

	private static void PrintMap(string mapName, int[,] map) {
		Debug.Log (mapName + " is:");
		for (int y = gridCount - 1; y >= 0; y--) {
			string message = string.Format("{0,3}",y) + ": ";
			string[] xArray = new string[gridCount];
			for (int x = 0; x < gridCount; x++) {
//				Debug.Log ("y is: " + y);
//				Debug.Log ("x is: " + x);
				xArray [x] = string.Format("{0,2}",map [x, y].ToString());
			}
			message = message + string.Join (", ", xArray);
			Debug.Log (message);
		}
	}
		
	private static void SetNextPlayer() {
		Debug.Log ("player was " + currentPlayer);
		currentPlayer = GetOpponent();
		SetCorrectPassButton ();
		Debug.Log ("next player is " + currentPlayer);
	}

	private static int GetOpponent() {
		return (currentPlayer + 1) % 2;
	}

	private static int GetOpponent(int player) {
		return (player + 1) % 2;
	}

	public static bool IsAllowed(int[] coordinates) {
		if (blockedCells == null) {
			return true;
		}
		for (int i = 0; i < blockedCells.Length; i++) {
			if (blockedCells [i].x == coordinates[0] && blockedCells [i].y == coordinates[1]) {
				Debug.Log ("not allowed here");
				return false;
			}
		}
		return GameScore.playerControlMap[coordinates[0], coordinates[1]] == GameOptions.NO_PLAYER;
	}

	private static void MarkCellsAsBlocked(Vector2[] cellArray) {
		blockedCells = cellArray;
	}

	public static int GetCurrentPlayer() {
		return currentPlayer;
	}

	private static void CaptureArray(Vector2[] coordinateArray, int player, int capturingPlayer) {
		Debug.Log ("CaptureArray called");

			
		foreach (Vector2 coordinates in coordinateArray) {
			if (OutOfBounds (Utils.Vector2ToIntArray(coordinates))) {
				Debug.Log ("cell is out of bounds");
				continue;
			}

			Capture (coordinates, player, capturingPlayer);
		}
	}

	private static void Capture(Vector2 coordinates, int player, int capturingPlayer) {
		Debug.Log ("Capture called");

		int[] coordinateInts = Utils.Vector2ToIntArray (coordinates);
		if (GameScore.playerControlMap[coordinateInts[0], coordinateInts[1]] != player) {
			// stone doesn't belong to the player in question, finished checking
			return;
		}

		markedCells = new LinkedList<CellData>();
		markedCellCount = 0;
		if (IsChainCaptured (coordinates , player, capturingPlayer)) {
			Debug.Log ("chain is captured");
			Debug.Log ("marked cells are " + markedCells.ToString());
			int scoreToAdd = 0;
			foreach (CellData markedCell in markedCells) {
				scoreToAdd++;

				// reset cellData object (also destroys the stone)
				markedCell.SetPlayer(GameOptions.NO_PLAYER);

				// remove player from playerControlMap
				GameScore.playerControlMap[markedCell.GetX(),markedCell.GetY()] = GameOptions.NO_PLAYER;
			}

			// add score for captured stones
			GameScore.AddScore (scoreToAdd, capturingPlayer);
		}
	}


	private static bool IsChainCaptured(Vector2 cell, int player, int capturingPlayer) {
		bool result = true;
		int[] coordinates = Utils.Vector2ToIntArray (cell);

		if (OutOfBounds (coordinates)) {
			Debug.Log ("cell is out of bounds");
			return true;
		}
		if (GameScore.chainMap[coordinates[0], coordinates[1]]) {
			// cell already checked
			return true;
		}

		// flag cell as checked
		GameScore.chainMap[coordinates[0], coordinates[1]] = true;

		// check cell
		Vector2[] adjacentCells = new Vector2[] {
			new Vector2(coordinates[0], coordinates[1] + 1),
			new Vector2(coordinates[0] - 1, coordinates[1]),
			new Vector2(coordinates[0], coordinates[1] - 1),
			new Vector2(coordinates[0] + 1, coordinates[1])};


		foreach (Vector2 adjacentCell in adjacentCells) {
			if (adjacentCell.x < 0 || adjacentCell.x >= gridCount || 
				adjacentCell.y < 0 || adjacentCell.y >= gridCount) {
				// we crossed the board bounds
				continue;
			}
			int[] adjacentCoordinates = Utils.Vector2ToIntArray (adjacentCell);
			if (GameScore.playerControlMap[adjacentCoordinates[0],adjacentCoordinates[1]] != GameOptions.NO_PLAYER) {
				// we found an opening - chain is not captured
//				markedCells = new LinkedList<int[]>(); // no need for this
//				markedCellCount = 0;
				PrintMap ("empty space chainMap", GameScore.chainMap);
				result = false;
				// remove all marked cells
				markedCells = new LinkedList<CellData>();
				markedCellCount = 0;

				break;
			}
			// if we find an adjacent cell with the opponent's stone, check it as well
			if (GameScore.playerControlMap[adjacentCoordinates[0],adjacentCoordinates[1]] == player && 
				!IsChainCaptured(adjacentCell, player, capturingPlayer)) { 
				// check of next stone resulted in false - pass on up the result
				PrintMap ("!IsChainCaptured. chainMap", GameScore.chainMap);
				result = false;
				break;
			}
		}

		if (result == true) {
			// this might be part of a captured chain, build a captured cell array
//			Debug.Log("cell is " + cell);
			CellData cellData = BoardController.GetCellDataFromVector2(cell);
			markedCells.AddLast(cellData);
			markedCellCount++;
		}

		// remove flag on the way out
		GameScore.chainMap[coordinates[0], coordinates[1]] = false;
		return result;
	}

	private static bool OutOfBounds(int[] coordinates) {
		return (coordinates [0] < 0 || coordinates [0] >= gridCount ||
			coordinates [1] < 0 || coordinates [1] >= gridCount);
	}

	public static void PassTurn() {
		Debug.Log("PassTurn");
		if (lastMoveWasPassed) {
			// opponent already passed his turn, game over
			EndTheGame();
			return;
		}
		lastMoveWasPassed = true;
		SetNextPlayer();
	}

	private static void EndTheGame() {
		Debug.Log("EndTheGame");

		// count all empty spaces and score the players
		for (int x = 0 ; x < gridCount ; x++) {
			for (int y = 0 ; y < gridCount ; y++) {	
				int player;
				bool foundCapturingPlayer;
				FindCapturingPlayer (x, y, out player, out foundCapturingPlayer);
				Capture (new Vector2(x, y), GameOptions.NO_PLAYER, player);
			}
		}
		// declare winner
	}
		
	private static void FindCapturingPlayer(int x, int y, out int player, out bool foundCapturingPlayer) {
		// set default values
		foundCapturingPlayer = false;
		player = GameOptions.NO_PLAYER;
		Vector2[] adjacentCells = new Vector2[] {
			new Vector2(x - 1, y), new Vector2(x, y - 1), new Vector2(x + 1, y), new Vector2(x, y + 1)};

		foreach (Vector2 adjacent in adjacentCells) {
			int[] adjacentInts = Utils.Vector2ToIntArray (adjacent);
			if (GameScore.playerControlMap[adjacentInts[0], adjacentInts[1]] > GameOptions.NO_PLAYER) { 
				// there's a stone on this cell 
				foundCapturingPlayer = true;
				player = GameScore.playerControlMap [adjacentInts[0], adjacentInts[1]];
				Debug.Log ("found capturing player on cell [" + adjacentInts[0] + "," + adjacentInts[1] + "]. " +
					"player is " + player + ".");
				return;
			}
		}
	}

	private static void IsCapturedEmptyChain() {
		// count all empty spaces and score the players
		// declare winner
		Debug.Log("EndTheGame");
	}

	private static void SetCorrectPassButton() {
		Debug.Log("SetCorrectPassButton");
		if (currentPlayer == GameOptions.player1) {
			panelController.player1PassButton.SetActive (true);
			panelController.player2PassButton.SetActive (false);
		} else if (currentPlayer == GameOptions.player2) {
			panelController.player1PassButton.SetActive (false);
			panelController.player2PassButton.SetActive (true);
		}
	}

}
