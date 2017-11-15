using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public float acceleration;
	public float maxSpeed;
	public float verticalSpeed;
	public float drag;
	public float minAngle;
	public float maxAngle;
	public float threshold;

	private Vector2 speed;
	private float zoom;
	private float axisHorizontal;
	private float axisVertical;

	void Start () {
		zoom = 20;
	}
		
	void FixedUpdate () {
		// get the input value
		axisHorizontal = Input.GetAxis("Horizontal");
		axisVertical = Input.GetAxis("Vertical");

		setNewRotationAndPosition ();
	}


	void setNewRotationAndPosition() {
		// add acceleration to current speed
		speed = new Vector2 (
			axisVertical * verticalSpeed,
			Mathf.Clamp (speed.y - (axisHorizontal * acceleration) - (speed.y * drag), -maxSpeed, maxSpeed));
//			axisHorizontal * verticalSpeed);
//			axisHorizontal * -1) * acceleration) +
//			(new Vector2 (-speed.x, -speed.y) * drag);
		// reduce drag from current speed
		if (Mathf.Abs (speed.x) < threshold) {
			speed.x = 0;
		}
		if (Mathf.Abs (speed.y) < threshold) {
			speed.y = 0;
		}
//		speed = new Vector2 (
//			Mathf.Clamp(speed.x, -maxSpeed, maxSpeed),
//			Mathf.Clamp(speed.y, -maxSpeed, maxSpeed));
		// rotate by delta speed
		Vector3 rotate = new Vector3 (speed.x, speed.y, 0.0f);
		transform.localEulerAngles = transform.localEulerAngles + rotate;
//		if (transform.localEulerAngles.x > 70) {
		transform.localEulerAngles = new Vector3(
			Mathf.Clamp(transform.localEulerAngles.x, minAngle, maxAngle),
			transform.localEulerAngles.y, 
			transform.localEulerAngles.z);
//		} else if (transform.localEulerAngles.x < 20) {
//			transform.localEulerAngles.x = 20.0f;
//		}
//		Debug.Log ("euler: " + transform.localEulerAngles);

		transform.localPosition = Vector3.zero;
		transform.localPosition += transform.forward * -zoom;
//		Debug.Log ("position: " + transform.position);
	}


}
