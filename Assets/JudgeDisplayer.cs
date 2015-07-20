using UnityEngine;
using System.Collections;

public class JudgeDisplayer : MonoBehaviour {

	public float CurJudge;

	// Use this for initialization
	void Start() {
		CurJudge = 0.0f;
		this.gameObject.GetComponent<TextMesh>().text = "NONE\n0.0%";
	}
	
	// Update is called once per frame
	void Update() {
	}

	public void SetJudge(float data, Note.Button but) {
		CurJudge = data;
		this.gameObject.GetComponent<TextMesh>().text
			= but.ToString() + '\n' + CurJudge.ToString("0.0") + '%';
	}
}
