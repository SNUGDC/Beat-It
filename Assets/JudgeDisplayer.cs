using UnityEngine;
using System.Collections;

public class JudgeDisplayer : MonoBehaviour {

	void Start() {
		this.gameObject.GetComponent<TextMesh>().text = "NONE\n0.0%";
	}
	
	void Update() {
	}

	public void SetJudge(float data, Note.Button but) {
		this.gameObject.GetComponent<TextMesh>().text
			= but.ToString() + '\n' + data.ToString("0.0") + '%';
	}
}
