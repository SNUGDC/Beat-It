using UnityEngine;

public class Turnline {
	public bool IsValid;
	public int Time;
	public Turnline(int time) {
		IsValid = false;
		Time = time;
	}
	
	public void Appear(Transform notePrefab) {
		Transform newTrans = Object.Instantiate(
			notePrefab, 
			new Vector3(0, -19.72f, -1.0f),
			Quaternion.identity
		) as Transform;
		IsValid = true;
		newTrans.parent = GameObject.Find("BeatGenerator").transform;
	}
}