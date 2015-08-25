using UnityEngine;
using System;
using System.Collections;
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
		ReadBeatFromTxt("exp2_easy");
		// generate test data
		/*for(int i = 1; i <= 100; ++i) {
			Note newNote = new Note((uint)i, 1000000 * (i + 3), (i % 7) == 0);
			NoteList.Enqueue(newNote);
		}*/
		_StartTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000);
        StartCoroutine("StartSong");
	}

    IEnumerator StartSong() {
        yield return new WaitForSeconds(NoteMover.NoteDelay);
        GameObject.Find("MusicPlayer").GetComponent<AudioSource>().Play();
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

    private void ReadBeatFromTxt(string fileName)
    {
        string noteString = Resources.Load<TextAsset>(fileName).text;
        string[] lines = noteString.Split('\n');

        int bpm = Int32.Parse(lines[1]);
        int quantize = Int32.Parse(lines[2]);
        float beatLen = 60.0f / ((float)bpm) * (4.0f / ((float)quantize));

        uint count = 0;
        uint notecount = 0;
        for (int i = 3; i < lines.Length; i++)
        {
            foreach (char c in lines[i])
            {
                switch (c)
                {
                    case '0':
                        notecount++;
                        Note newNote = new Note(notecount, Convert.ToInt32(count * beatLen * 1000000) + BEAT_DELAY, false);
                        NoteList.Enqueue(newNote);
                        count++;
                        break;
                    case '-':
                        count++; break;
                    case ' ':
                        break;
                    case ';':
                        NoteList.Last().Flip = true; break;
                }
            }
        }
    }
}