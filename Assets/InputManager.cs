using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	public BeatGenerator BeatGen;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		int curTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000);
		// find first unpressed note in VALID State for player 1
		foreach(Note n in BeatGen.NoteList) {
			if(n.IsValid && n.PressedButton[0] == Note.Button.NONE) {
				// found target note
				// check input & set button for player 1
				if(Input.GetKeyDown(ButToKeycode(0, Note.Button.RED)))
					n.Press(0, curTime - BeatGen.StartTime, Note.Button.RED);
				else if(Input.GetKeyDown(ButToKeycode(0, Note.Button.BLUE)))
					n.Press(0, curTime - BeatGen.StartTime, Note.Button.BLUE);
				else if(Input.GetKeyDown(ButToKeycode(0, Note.Button.GREEN)))
					n.Press(0, curTime - BeatGen.StartTime, Note.Button.GREEN);
				break;
			}
		}
		// find first unpressed note in VALID State for player 2
		foreach(Note n in BeatGen.NoteList) {
			if(n.IsValid && n.PressedButton[1] == Note.Button.NONE) {
				// found target note
				// check input & set button for player 2
				if(Input.GetKeyDown(ButToKeycode(1, Note.Button.RED)))
					n.Press(1, curTime - BeatGen.StartTime, Note.Button.RED);
				else if(Input.GetKeyDown(ButToKeycode(1, Note.Button.BLUE)))
					n.Press(1, curTime - BeatGen.StartTime, Note.Button.BLUE);
				else if(Input.GetKeyDown(ButToKeycode(1, Note.Button.GREEN)))
					n.Press(1, curTime - BeatGen.StartTime, Note.Button.GREEN);
				break;
			}
		}
	}

	private KeyCode ButToKeycode(int player, Note.Button but) {
		switch(but) {
			case Note.Button.RED :
				return (player == 0) ? KeyCode.A : KeyCode.J;
			case Note.Button.BLUE :
				return (player == 0) ? KeyCode.S : KeyCode.K;
			case Note.Button.GREEN :
				return (player == 0) ? KeyCode.D : KeyCode.L;
			default :
				return KeyCode.None;
		}
	}

	public void Ready() {
		
	}
}