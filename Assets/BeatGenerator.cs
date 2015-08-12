using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using SimpleJSON;

public class BeatGenerator : MonoBehaviour {
	public const int BEAT_DELAY = 3000000;
	public const uint BEAT_MARGIN = 500000; // time margin for judge

	public BattleManager BattleManager; // BattleManager object
	public Transform NotePrefab; // prefab for note
	public Queue<Note> NoteList = new Queue<Note>(); // List of Notes
	public int StartTime {
		get { return this._StartTime; }
	}

	private int _StartTime; // Starting time of battle

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
		// load note data from json
		string jsonString = Resources.Load<TextAsset>("test_data").text;
		var data = JSON.Parse(jsonString);
		int noteCount = data["notecount"].AsInt;
		for(int i = 0; i < noteCount; i++) {
			Note newNote = new Note(
				(uint)i,
				data["notes"][i]["time"].AsInt + BEAT_DELAY
			);
			newNote.Flip = data["notes"][i]["flip"].AsBool;
			NoteList.Enqueue(newNote);
		}
		_StartTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000);
		GameObject.Find("NetworkManager").GetComponent<NetworkConnector>()
			.OnRead = this.OnSocketRead;
	}

	void Update() {
		// Dequeue & kill last note
		int curTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000);
		// find first note in READY State & show at appropriate time
		foreach(Note n in NoteList) {
			if(!n.IsValid) {
				// found target note
				if(curTime - this.StartTime
				   > n.Time - NoteMover.NoteDelay * 1000000) {
					n.Appear(NotePrefab);
				}
				break;
			}
		}
		Note curNote = NoteList.Peek();
		if(curNote != null && curTime >= curNote.Time + this.StartTime
										 + BeatGenerator.BEAT_MARGIN) {
			this.GetComponent<AudioSource>().Play();
			curNote = this.NoteList.Dequeue();
			curNote.Kill();
		}
	}
}