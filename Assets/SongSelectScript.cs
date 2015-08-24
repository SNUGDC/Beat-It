using UnityEngine;
using System.Collections;


public class SongSelectScript : MonoBehaviour {
	
	// TODO: selecting song
	void Update () {
		if (Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.K)) {
		}
		else if (Input.GetKeyDown (KeyCode.Return))
			Application.LoadLevel ("sceneBattle");
	}
}
