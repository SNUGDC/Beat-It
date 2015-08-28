using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class SongSelectScript : MonoBehaviour {

	public PlayerScript player1;
	public PlayerScript player2;
	public GameData gameData;

	void Update () {
		if (player1.isReady && player2.isReady) {
			gameData.SaveGameData();
			Application.LoadLevel ("sceneBattle");
		}
	}
}
