using UnityEngine;
using System.Collections;

public class NoteMover : MonoBehaviour {
	public const float NoteDelay = 3.0f;

	public Note NoteData;
	public GameObject LeftObject;
	public GameObject RightObject;

	private bool IsMoving;

	void Start() {
		IsMoving = true;
	}
	
	void Update() {
		if(IsMoving) {
			float speed = Camera.main.orthographicSize * 16 / 9
						  / NoteMover.NoteDelay * Time.deltaTime;
			LeftObject.transform.Translate(speed, 0, 0);
			RightObject.transform.Translate(-speed, 0, 0);
			// delete note from display
			if(LeftObject.transform.position.x >= 0) {
				IsMoving = false;
				Object.Destroy(this.gameObject);
			}
		}
	}
}