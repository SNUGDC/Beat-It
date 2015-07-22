using UnityEngine;
using System;
using System.Collections.Generic;

public class BeatGenerator : MonoBehaviour {
	public Transform NotePrefab;
	public BattleManager BattleManager;

	public Queue<Note> NoteList = new Queue<Note>();
	private int StartTime; // total time passed since start of music

	public void OnSocketRead(string input) {
		// socket format is
		// "player noteId judge button"
		string[] splitInput = input.Split(' ');
		int player = Convert.ToInt32(splitInput[0]);
		BattleManager.Data newData = new BattleManager.Data {
			Id = Convert.ToUInt32(splitInput[1]),
			Judge = Convert.ToUInt32(splitInput[2]),
			Button = (Note.Button)Enum.Parse(typeof(Note.Button),
													splitInput[3])
		};
		BattleManager.DataQueue[player].Enqueue(newData);
	}

	void Start() {
		// TODO : read beat data from file
		for(int i = 1; i <= 100; ++i) {
			Note newNote = new Note((uint)i, 1000000 * (i + 3));
			//if(i % 5 == 0) newNote.Flip = true;
			NoteList.Enqueue(newNote);
		}
		StartTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000);
		GameObject.Find("NetworkManager").GetComponent<NetworkConnector>()
			.OnRead = this.OnSocketRead;
	}

	void Update() {
		int CurTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000);
		// TODO : add loading beats from queue
		// find first note in READY State & show at appropriate time
		foreach(Note n in NoteList) {
			if(!n.IsValid) {
				// found target note
				if(CurTime - StartTime
				   > n.Time - NoteMover.NoteDelay * 1000000) {
					n.Appear(this, NotePrefab);
				}
				break;
			}
		}
		// find first unpressed note in VALID State for player 1
		foreach(Note n in NoteList) {
			if(n.IsValid && n.PressedButton[0] == Note.Button.NONE) {
				// found target note
				// check input & set button for player 1
				if(Input.GetKeyDown(Note.ButtonTable[0, (int)Note.Button.RED]))
					n.Press(0, CurTime - StartTime, Note.Button.RED);
				else if(Input.GetKeyDown(Note.ButtonTable[0, (int)Note.Button.BLUE]))
					n.Press(0, CurTime - StartTime, Note.Button.BLUE);
				else if(Input.GetKeyDown(Note.ButtonTable[0, (int)Note.Button.GREEN]))
					n.Press(0, CurTime - StartTime, Note.Button.GREEN);
				break;
			}
		}
		// find first unpressed note in VALID State for player 2
		foreach(Note n in NoteList) {
			if(n.IsValid && n.PressedButton[1] == Note.Button.NONE) {
				// found target note
				// check input & set button for player 2
				if(Input.GetKeyDown(Note.ButtonTable[1, (int)Note.Button.RED]))
					n.Press(1, CurTime - StartTime, Note.Button.RED);
				else if(Input.GetKeyDown(Note.ButtonTable[1, (int)Note.Button.BLUE]))
					n.Press(1, CurTime - StartTime, Note.Button.BLUE);
				else if(Input.GetKeyDown(Note.ButtonTable[1, (int)Note.Button.GREEN]))
					n.Press(1, CurTime - StartTime, Note.Button.GREEN);
				break;
			}
		}
	}
}