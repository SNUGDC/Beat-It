using UnityEngine;
using System.Collections;

public class CharacterData {
	public Character character;
	public int playerID;
	public CharacterData (Character character_t, int playerID_t) {
		character = character_t;
		playerID = playerID_t;
	}
}

public class GameData : MonoBehaviour {

	[HideInInspector] public CharacterData[] characterData;
	[HideInInspector] public Song song;
	public PlayerScript char1, char2;
	// Use this for initialization

	void Awake() {
		DontDestroyOnLoad (transform.gameObject);
	}

	public void SaveGameData() {
		characterData = new CharacterData[2] {
			new CharacterData (char1.character, char1.playerID),
			new CharacterData (char2.character, char2.playerID)
		};
		song = GameObject.Find ("Record").GetComponent<RecordScript> ().GetCurrentSong ();
	}
}