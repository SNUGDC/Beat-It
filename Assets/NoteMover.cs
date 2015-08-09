using UnityEngine;
using System.Collections;

public class NoteMover : MonoBehaviour {
	public const float NoteDelay = 3.0f;
	public Note NoteData;
	public GameObject LeftObject;
	public GameObject RightObject;

	private bool isMoving;

	void Start() {
		isMoving = true;
	}
	
	void Update() {
		if(isMoving) {
			float speed = Camera.main.orthographicSize * 16 / 9
						  / NoteMover.NoteDelay * Time.deltaTime;
			LeftObject.transform.Translate(speed, 0, 0);
			RightObject.transform.Translate(-speed, 0, 0);
			// delete note from display
			if(LeftObject.transform.position.x >= 0) {
				isMoving = false;
				this.GetComponent<AudioSource>().Play();
			}
		}
		else {
			if(!this.GetComponent<AudioSource>().isPlaying) {
				NoteData.Kill();
				Object.Destroy(this.gameObject);
				transform.parent.GetComponent<BeatGenerator>()
					.NoteList.Dequeue();
			}
		}
	}
}
