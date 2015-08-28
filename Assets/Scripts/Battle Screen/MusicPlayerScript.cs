using UnityEngine;
using System.Collections;

public class MusicPlayerScript : MonoBehaviour {

	void Start()
	{
		var gameData = GameObject.Find ("GameData").GetComponent<GameData> ();
		GetComponent<AudioSource> ().clip = gameData.song.music;
	}
}
