using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightController : MonoBehaviour {

	private readonly float fullIntensity = 10;
	private Vector3 zeroPosition;
	private Light spotlight;

	void Awake() {
		spotlight = GetComponent<Light> ();
		Debug.Log ("SpotlightController - spotlight is " + spotlight);
		zeroPosition = spotlight.transform.position;
	}

	public void SelectCell(int x, int y) {
		transform.position = new Vector3 (
			zeroPosition.x + x,
			zeroPosition.y,
			zeroPosition.z + y);
		spotlight.intensity = fullIntensity;
	}

	public void Deselect() {
		spotlight.intensity = 0;
	}

}
