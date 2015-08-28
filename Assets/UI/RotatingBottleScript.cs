using UnityEngine;
using System.Collections;

public class RotatingBottleScript : MonoBehaviour {

	private float timeSinceLoad = 0.0f;
	
	private float speed = 1000.0f;
	private float deceleration = 1000.0f;

	private bool stopping = false;
	void Start() {
		StartCoroutine (StartStopping ());
	}
	void Update () {
		timeSinceLoad += Time.deltaTime;
		if (stopping) {
			speed -= deceleration * Time.deltaTime;
			if (speed < 0) {
				// change attack/defend depending on bottle position
				var battleManager = GameObject.Find ("BattleManager").GetComponent<BattleManager>();
				if ((transform.rotation.z % 360.0f) < 180.0f) {
					battleManager.FlippAttackerOnStart();
				}
				// prepare the notes
				var beatGenerator = GameObject.Find ("BeatGenerator").GetComponent<BeatGenerator>();
				foreach(Note note in beatGenerator.NoteList) {
					note.Time += Mathf.RoundToInt(timeSinceLoad*1000000);
					note.Time -= beatGenerator.BEAT_DELAY_TEMP - (int)NoteMover.NoteDelay * 1000000;
				}
				// initiate countdown
				GameObject.Find ("CountdownText").GetComponent<CountdownScript>().startCountdown = true;
				stopping = false;
				speed = 0;
				Destroy(gameObject);
			}
		}
		transform.Rotate (new Vector3 (0f, 0f, speed * Time.deltaTime));
	}

	IEnumerator StartStopping()
	{
		yield return new WaitForSeconds(Random.Range (0.0f, 7.0f * 360.0f / speed));
		stopping = true;
	}

}
