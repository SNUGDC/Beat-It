using UnityEngine;
using System.Collections;

public class TurnlineMover : MonoBehaviour {
	public const float NoteDelay = 3.0f;
	public GameObject LeftObject;
	public GameObject RightObject;
	
	private bool IsMoving = true;
	
	void Update() {
		if(!IsMoving) return;
		float speed = Camera.main.orthographicSize * 16 / 9 
			/ NoteMover.NoteDelay * Time.deltaTime;
		LeftObject.transform.Translate(speed, 0, 0);
		RightObject.transform.Translate(-speed, 0, 0);
		if(LeftObject.transform.position.x >= 0) {
			StartCoroutine(Kill());
			IsMoving = false;
		}
	}

	IEnumerator Kill() {
		// delete note from display
		const float delay = 0.4f;
		Object.Destroy(LeftObject);
		Object.Destroy(RightObject);
		yield return new WaitForSeconds(delay);
		Object.Destroy(this.gameObject);
	}
}
