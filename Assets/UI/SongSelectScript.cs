using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class SongSelectScript : MonoBehaviour {

	public PlayerScript player1;
	public PlayerScript player2;
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return) && player1.isReady && player2.isReady)
			Application.LoadLevel ("sceneBattle");
	}
}
