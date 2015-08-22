using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using SimpleJSON;

public class BeatGenerator : MonoBehaviour {
	public const int BEAT_DELAY = 3000000;
	public const uint BEAT_MARGIN = 500000; // time margin for judge

	public Transform NotePrefab; // prefab for note
	public Queue<Note> NoteList = new Queue<Note>(); // List of Notes
	public int StartTime {
		get { return _StartTime; }
	}

	private int _StartTime; // Starting time of battle

	void Start() {
		// ReadBeat("test_data");
		// generate test data
		for(int i = 1; i <= 100; ++i) {
			Note newNote = new Note((uint)i, 2000000 * (i + 3), (i % 7) == 0);
			NoteList.Enqueue(newNote);
		}
		_StartTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000);
	}

	void Update() {
		// Dequeue & kill last note
		int curTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000);
		// find first note in READY State & show at appropriate time
		Note appearNote = NoteList.First(n => !n.IsValid);
		if(appearNote != null && curTime - this.StartTime > appearNote.Time - BEAT_DELAY) {
			appearNote.Appear(NotePrefab);
		}
		// play sound & kill note
		Note curNote = NoteList.Peek();
		if(curNote != null && curTime >= curNote.Time + this.StartTime) {
			this.GetComponent<AudioSource>().Play();
			NoteList.Dequeue().Kill();
		}
	}

	private void ReadBeat(string fileName) {
		// load note data from json
		string jsonString = Resources.Load<TextAsset>(fileName).text;
		var data = JSON.Parse(jsonString);
		int noteCount = data["notecount"].AsInt;
		for(int i = 0; i < noteCount; i++) {
			var curData = data["notes"][i];
			Note newNote = new Note((uint)i,
									curData["time"].AsInt + BEAT_DELAY,
									curData["flip"].AsBool);
			NoteList.Enqueue(newNote);
		}
	}
}