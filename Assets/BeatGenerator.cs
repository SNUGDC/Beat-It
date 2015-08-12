using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using SimpleJSON;

public class BeatGenerator : MonoBehaviour {
	public const int BEAT_DELAY = 3000000;
	public Transform NotePrefab; // prefab for note
	public BattleManager BattleManager; // BattleManager object

	public Queue<Note> NoteList = new Queue<Note>(); // List of Notes
	private int StartTime; // Starting time of battle

	public int bpm; // bpm of song

	// interpret input & put data into queue
	public void OnSocketRead(string input) {
		// message format is
		// "player noteId judge button"
		string[] splitInput = input.Split(' ');
		int player = Convert.ToInt32(splitInput[0]);
		BattleManager.Data newData = new BattleManager.Data {
			Id = Convert.ToUInt32(splitInput[1]),
			Judge = Convert.ToUInt32(splitInput[2]),
			Button = (Note.Button)Enum.Parse(typeof(Note.Button),
													splitInput[3])
		};
		// add interpreted data into data queue
		BattleManager.DataQueue[player].Enqueue(newData);
	}

	void Start() {
		LoadFromTxt ("test_data");
		StartTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000);
		GameObject.Find("NetworkManager").GetComponent<NetworkConnector>()
			.OnRead = this.OnSocketRead;
	}

	void LoadDefaultBeat() {
		for(int i = 0; i <= 100; ++i) {
			Note newNote = new Note((uint)i, 1500000 * (i + 3));
			if(i % 7 == 0) newNote.Flip = true;
			NoteList.Enqueue(newNote);
		}
	}
	// load from json file
	void LoadFromJSON(string txtpath) {
		string jsonString = Resources.Load<TextAsset>(txtpath).text;
		var data = JSON.Parse (jsonString);
		int noteCount = data["notecount"].AsInt;
		Debug.Log (noteCount);
		for (int i = 0; i < noteCount; i++) {
			Note newNote =
				new Note((uint)i,
				         data["notes"][i]["time"].AsInt + BEAT_DELAY);
			newNote.Flip = data["notes"][i]["flip"].AsBool;
			NoteList.Enqueue(newNote);
		}
	}
	// load from txt file
	void LoadFromTxt(string txtpath) {
		string noteString = Resources.Load<TextAsset>(txtpath).text;
		string[] lines = noteString.Split('\n');

		int bpm = Int32.Parse (lines[1]);
		int quantize = Int32.Parse (lines[2]);
		float beatLen = 60.0f / ((float)bpm) * (4.0f/((float)quantize));

		uint count = 0;
		uint notecount = 0;
		for(int i = 3; i < lines.Length; i++) {
			foreach (char c in lines[i]) {
				switch(c)
				{
				case '0':
					notecount++;
					Note newNote = new Note(notecount, Convert.ToInt32 (count * beatLen * 1000000) + BEAT_DELAY);
					newNote.Flip = false;
					NoteList.Enqueue(newNote);
					count++;
					break;
				case '-':
					count++; break;
				case ' ':
					break;
				case ';':
					NoteList.Last ().Flip = true; break;
				}
			}
		}
	}

	void Update() {
		// calculate current time
		int CurTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000);
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