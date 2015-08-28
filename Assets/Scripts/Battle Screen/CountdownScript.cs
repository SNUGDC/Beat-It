using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownScript : MonoBehaviour {

	public float time = 3.0f;

	[HideInInspector] public bool startCountdown = false;

	private Text countdownText;
	public BeatGenerator bg;

	void Start() {
		countdownText = GetComponent<Text> ();
		countdownText.text = "";
	}
	void Update() {
		if (startCountdown) {
			countdownText.text = (Mathf.Ceil (time)).ToString();
			time -= Time.deltaTime;
			if (time < 0) {
				// start the whole game
				GameObject.Find ("MusicPlayer").GetComponent<AudioSource>().PlayDelayed(NoteMover.NoteDelay);
				startCountdown = false;
				Destroy (gameObject);
			}
		}

	}
}
