using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellData : MonoBehaviour {

	private static SpotlightController spotlightController;

	public GameObject stonePrefabWhite;
	public GameObject stonePrefabBlack;
	private GameObject cell;
	private GameObject stone;
	private Renderer render;
	private int x;
	private int y;
	private int player = GameOptions.NO_PLAYER;

	void Awake() {

//		Debug.Log ("setting up spotlight...");

		Light spotlight = GameObject.FindGameObjectWithTag ("Spotlight").GetComponent<Light>();
//		Debug.Log ("spotlight is " + spotlight);
		spotlightController = spotlight.GetComponent<SpotlightController> ();

//		Debug.Log ("spotlight controller is " + spotlightController);
	}

	public CellData () {}

	public CellData (int x, int y, GameObject cell) {
//		if (spotlightController == null) {
//			spotlightController = GameObject.FindGameObjectWithTag ("Spotlight").
//				GetComponent<SpotlightController> ();
//		}
		this.x = x;
		this.y = y;
		this.cell = cell;
	}

	public void Init(int x, int y, GameObject cell) {
		this.x = x;
		this.y = y;
		this.cell = cell;
		render = cell.GetComponent<Renderer> ();
		if (GameOptions.debug) {
			render = cell.GetComponentInChildren<Renderer> ();
			render.enabled = true;
			colorCell (GameOptions.NO_PLAYER);
		}
	}

	public void SetPlayer(int player) {
		this.player = player;
		if (player == GameOptions.NO_PLAYER) {
			// remove stone
			DestroyStone ();
		} else {
			// add stone to cell
			AddStone ();
		}
	}
		
	public int GetPlayer() {
		return player;
	}

	private void AddStone() {
		// code to add the stone to the board
		Debug.Log ("entered addStone");
		GameObject stonePrefab = (Rules.GetCurrentPlayer () == GameOptions.WHITE ? 
			stonePrefabWhite : stonePrefabBlack);
		stone = GameObject.Instantiate(stonePrefab, cell.transform);
		Deselect ();
	}

	private void DestroyStone() {
		// code to remove the stone from the board
		Destroy(stone);
	}

	public void Select() {
		// highlight the cell
//		Debug.Log ("entered select");
		if (player == GameOptions.NO_PLAYER) {
			spotlightController.SelectCell (x, y);
		} else {
			Deselect ();
		}
	}

	public void Deselect() {
		// remove the cell's highlight
		spotlightController.Deselect ();

//		Light light = cell.GetComponentInChildren<Light>();
//		light.intensity = 0;
	}

	public Vector2 GetV2() {
		return new Vector2 (x, y);
	}

	public int GetX() {
		return x;
	}

	public int GetY() {
		return y;
	}

	public bool PositionEquals(GameObject otherCell) {
		if (otherCell == null) {
//			Debug.Log ("positionEquals called. other cell game object is null.");
			return false;
		}
		CellData otherCellData = otherCell.GetComponent<CellData> ();
//		Debug.Log ("positionEquals called. position is [" + x + "," + y + "]" +
//			", other position is [" + otherCellData.x + "," + otherCellData.y + "]");
		return (otherCellData != null && otherCellData.x == x && otherCellData.y == y);
	}




	// debug - color cell by player control
	public void colorCell(int player) {
		if (player == GameOptions.NO_PLAYER) {
//			Destroy (render);
			render.material.color = Color.blue;
		} else if (player == GameOptions.player1) {
			render.material.color = Color.green;
		} else if (player == GameOptions.player2) {
			render.material.color = Color.red;
		}
//		render.material. = ;
	}

}
