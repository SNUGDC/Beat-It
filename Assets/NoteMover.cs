using UnityEngine;

public class NoteMover : MonoBehaviour {
	public const float NoteDelay = 3.0f;

	public Note NoteData;
	public GameObject LeftObject;
	public GameObject RightObject;

	void Start() {
	}
	
	void Update() {
		float speed = Camera.main.orthographicSize * 16 / 9
					  / NoteMover.NoteDelay * Time.deltaTime;
		LeftObject.transform.Translate(speed, 0, 0);
		RightObject.transform.Translate(-speed, 0, 0);
		// delete note from display
		if(LeftObject.transform.position.x >= 0) {
			Object.Destroy(this.gameObject);
		}
	}
}