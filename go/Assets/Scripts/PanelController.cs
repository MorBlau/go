using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour {

	public GameObject player1PassButton;
	public GameObject player2PassButton;
	public GameObject player1Score;
	public GameObject player2Score;
	public GameObject player1Name;
	public GameObject player2Name;

	private Text player1ScoreText = null;
	private Text player2ScoreText = null;
	private Text player1NameText = null;
	private Text player2NameText = null;

	public void SetNewScore(int player) {
		if (player1ScoreText == null || player2ScoreText == null) {
			InitTexts ();
		}
		Text playerScoreText = null;
		if (player == GameOptions.player1) {
			playerScoreText = player1ScoreText;
		} else if (player == GameOptions.player2) {
			playerScoreText = player2ScoreText;
		}
		int value = -1;
		GameScore.count.TryGetValue (player, out value);
		if (value > -1) {
			playerScoreText.text = string.Format("{0}", value);
		}
	}

	public void SetPlayerNames() {
		if (player1NameText == null || player2NameText == null) {
			InitTexts ();
		}
		player1NameText.text = GameOptions.player1Name;
		player2NameText.text = GameOptions.player2Name;
	}

	private void InitTexts () {
		player1ScoreText = player1Score.GetComponent<Text>();
		player2ScoreText = player2Score.GetComponent<Text>();
		player1NameText = player1Name.GetComponent<Text>();
		player2NameText = player2Name.GetComponent<Text>();
	}

}
