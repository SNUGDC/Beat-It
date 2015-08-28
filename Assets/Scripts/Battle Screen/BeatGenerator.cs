using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BeatGenerator : MonoBehaviour {
	public const uint BEAT_MARGIN = 500000; // time margin for judge
	public int BEAT_DELAY_TEMP = 100000000;

	public string SongName;
	public Transform NotePrefab; // prefab for note
	public Transform LinePrefab;
	public Queue<Note> NoteList = new Queue<Note>(); // List of Notes
	public Queue<Turnline> FlipList = new Queue<Turnline>();
	public int StartTime {
		get { return _StartTime; }
	}

	private int _StartTime; // Starting time of battle

	void Start() {
		ReadBeatFromTxt(GameObject.Find ("GameData").GetComponent<GameData> ().song.name);
		// generate test data
		/*for(int i = 1; i <= 100; ++i) {
			Note newNote = new Note((uint)i, 1000000 * (i + 3), (i % 7) == 0);
			NoteList.Enqueue(newNote);
		}*/
		_StartTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000);
	}

	void Update() {
		// Dequeue & kill last note
		int curTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000);
		// find first note in READY State & show at appropriate time
		Note appearNote = null;
		foreach(Note n in NoteList) {
			if(!n.IsValid) {
				appearNote = n;
				break;
			}
		}
		
		Turnline appearLine = null;
		foreach(Turnline l in FlipList) {
			if(!l.IsValid) {
				appearLine = l;
				break;
			}
		}
		if(appearNote != null && curTime - _StartTime > appearNote.Time - NoteMover.NoteDelay * 1000000) {
			appearNote.Appear(NotePrefab);
		}
		if(appearLine != null && curTime - _StartTime > appearLine.Time - TurnlineMover.NoteDelay * 1000000) {
			appearLine.Appear(LinePrefab);
		}
		// play sound & kill note
		if(NoteList.Count != 0 && curTime >= NoteList.Peek().Time + _StartTime) {
			NoteList.Dequeue().Kill();
		}
		if(FlipList.Count != 0 && curTime >= FlipList.Peek().Time + _StartTime) {
			GameObject.Find("BattleManager").GetComponent<BattleManager>().FlipAttacker();
			FlipList.Dequeue();
		}
	}

	private void ReadBeatFromTxt(string fileName) {
		string noteString = Resources.Load<TextAsset>(fileName).text;
		string[] lines = noteString.Split('\n');

		int bpm = Int32.Parse(lines[1]);
		int quantize = Int32.Parse(lines[2]);
		float beatLen = 60.0f / (float)bpm * (4.0f / (float)quantize);

		uint count = 0;
		uint notecount = 0;
		for (int i = 3; i < lines.Length; ++i) {
			foreach (char c in lines[i]) {
				switch (c) {
					case '0':
						notecount++;
						Note newNote
							= new Note(notecount,
									   (int)((count * beatLen + NoteMover.NoteDelay) * 1000000) + BEAT_DELAY_TEMP);
						NoteList.Enqueue(newNote);
						count++;
						break;
					case '-':
						count++; break;
					case ' ':
						break;
					case ';':
						Turnline newTurnline = new Turnline(
						(int)((count * beatLen + NoteMover.NoteDelay) * 1000000) + BEAT_DELAY_TEMP);
						FlipList.Enqueue(newTurnline);
						break;
				}
			}
		}
	}
}