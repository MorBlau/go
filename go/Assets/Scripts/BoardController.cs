using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BoardController : MonoBehaviour {

	public GameObject cellContainer;
	public GameObject cells;
	public GameObject player1ScoreText;
	public GameObject player2ScoreText;
	public GameObject player1PassButton;
	public GameObject player2PassButton;

	private Dictionary <Vector2, GameObject> gridMap;
	private SpotlightController spotlightController;
	private GameObject selectedCell;
	private bool isCellSelected = false;
	private bool alreadyClicked = false;
	private static BoardController _anInstance;

	void Awake() {
		_anInstance = this;
	}

	void Start () {

		GameScore.Init ();

		// rules should be initiated at the begining of a game
		Rules.Init();

		Light spotlight = GameObject.FindGameObjectWithTag ("Spotlight").GetComponent<Light>();
		spotlightController = spotlight.GetComponent<SpotlightController> ();

		gridMap = new Dictionary<Vector2, GameObject>();
		SpawnGridCells ();
		Rules.Init ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		HandleMouseInput (Input.GetMouseButton (0));	
	}

	void HandleMouseInput(bool clicked) {
		bool isMouseOverCell = false;
		if (clicked) {
//			Debug.Log ("Left mouse button pressed!");
		} else if (alreadyClicked) {
			alreadyClicked = false;
		}
		if (!Rules.processingMove && !alreadyClicked) { // if we are in the process of clicking from last frame, do nothing...
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {

				Vector3 point = hit.point;

				point = transform.InverseTransformPoint (point);
//				Debug.Log ("Mouse touching board at " + point);
				point = point - cells.transform.position;
				Vector2 point2D = new Vector2 (Mathf.RoundToInt (point.x),
					Mathf.RoundToInt (point.z));
//				Debug.Log ("Mouse touching board at (2D) " + point2D);
				GameObject touchedCell;
				CellData cellData;
				if (gridMap.TryGetValue (point2D, out touchedCell)) {
					isMouseOverCell = true;
					cellData = touchedCell.GetComponent<CellData> ();
					if (clicked) { // player clicked on cell
						// check if stone is allowed on cell
						if (Rules.IsAllowed (Utils.Vector2ToIntArray(point2D))) {
							ClickOnCell (touchedCell, cellData);
						}
					} else {
						HighlightCell (touchedCell, cellData);
					}
				}
			} 

		}
		if (!isMouseOverCell && isCellSelected) {
			// if no cell is touched, remove spotlight
//			Debug.Log("deselecting");
			spotlightController.Deselect ();
		}
		if (clicked) {
			alreadyClicked = true; // make click sticky
		}
	}
		
	void HighlightCell (GameObject touchedCell, CellData cellData) {

//			Debug.Log ("cell is " + touchedCell + ", selectedCell is " + selectedCell);

		if (cellData == null || cellData.PositionEquals (selectedCell)) {
			// do nothing
		} else {
			// mark the cell
//			Debug.Log ("selecting new cell");
			selectedCell = touchedCell;
			cellData.Select ();
			isCellSelected = true;
		}
	}

	void ClickOnCell(GameObject touchedCell, CellData cellData) {
		if (cellData.GetPlayer() == GameOptions.NO_PLAYER) {
			Rules.processingMove = true;
			// set player in cell data. This also adds a stone to cell.
			cellData.SetPlayer (Rules.GetCurrentPlayer ());


		} else {
			Debug.Log ("clicked on an occupied cell");
		}
	}

	void SpawnGridCells() {
		int count = GameOptions.GetGridCount ();
		for (int x = 0; x < count; x++) {
			for (int y = 0; y < count; y++) {
				GameObject spawnedCell = SpawnCell (x, y);
				CellData cellData = spawnedCell.GetComponent<CellData> ();
				cellData.Init(x, y, spawnedCell);
//				Debug.Log ("Grid map length is " + gridMap.Count);
				gridMap.Add (new Vector2 (x, y), spawnedCell);
//				Debug.Log ("Grid map at [" + x + "," + y + "] has " + spawnedCell);

			}
		}
	}

	GameObject SpawnCell(int x, int y) {
//		Debug.Log ("Spawning grid cell at [" + x + "," + y + "]");
		GameObject newCell = Instantiate(cellContainer, cells.transform);
		newCell.transform.localPosition = new Vector3 (x, y, 0);
		return newCell;
	}
		
	public static CellData GetCellDataFromVector2(Vector2 cellV2) {
		GameObject touchedCell;
		if (_anInstance.gridMap.TryGetValue (cellV2, out touchedCell)) {
			return touchedCell.GetComponent<CellData> ();
		}
		return null;
	}

	public static Dictionary <Vector2, GameObject> getGridMap() {
		return _anInstance.gridMap;
	}

//	public static GameObject getEventSystem() {
//		if (_anInstance.eventSystem == null) {
//			_anInstance.initEventSystem ();
//		}
//		return _anInstance.eventSystem;
//	}
//
	public static Text GetPlayer1ScoreText() {
		return _anInstance.player1ScoreText.GetComponent<Text>();
	}

	public static Text GetPlayer2ScoreText() {
		return _anInstance.player2ScoreText.GetComponent<Text>();
	}

	public void OnPassButtonPress() {
		Rules.PassTurn ();
	}

}
