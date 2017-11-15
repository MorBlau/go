using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellStatus : MonoBehaviour {

	public int playerControl = 0;

	// Use this for initialization
	void Start ()
	{
	}
		

	void OnCollisionEnter (Collision col)
	{
		Debug.Log ("collision");
		Rules.ProcessMove (GetComponentInParent<CellData>());
	}


	private void DestroyGracefully (GameObject gameObject, float timeToDie)
	{
		Renderer rend = gameObject.GetComponent<Renderer> ();
		StartCoroutine (FadeObject (rend, gameObject, timeToDie));
		MonoBehaviour.Destroy (gameObject, timeToDie);

	}

	private IEnumerator FadeObject (Renderer rend, GameObject gameObject, float timeToDie) 
	{
		Color startColor = rend.material.color;
		Color endColor = startColor;
		endColor.a = 0;

		float localTime = 0.0f;
		while (localTime < timeToDie) {
			float ratio = localTime / timeToDie;
			Color newColor = Color.Lerp (startColor, endColor, ratio);
			rend.material.SetColor("_Color", newColor);
			yield return new WaitForFixedUpdate();
			localTime += Time.deltaTime;
			Debug.Log ("delta time is: " + Time.deltaTime + ", local time is: " + localTime + 
				", color is: " + newColor);
		}


	}

}