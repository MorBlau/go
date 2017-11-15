using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour {

	public float speed;

	private float zoom;


	void Start () {
		zoom = 20;
	}


	void FixedUpdate () {
		setNewRotationAndPosition ();
	}


	void setNewRotationAndPosition() {
		Vector3 rotate = new Vector3 (0.0f, 1, 0.0f) * speed;
		transform.localEulerAngles = transform.localEulerAngles + rotate;
		//		if (transform.localEulerAngles.x > 70) {
		transform.localEulerAngles = new Vector3(
			Mathf.Clamp(transform.localEulerAngles.x, 20.0f, 70.0f),
			transform.localEulerAngles.y, 
			transform.localEulerAngles.z);

		transform.localPosition = Vector3.zero;
		transform.localPosition += transform.forward * -zoom;
	}

}
