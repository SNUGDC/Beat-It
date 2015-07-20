using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public GameObject HpBar;
	public GameObject SpBar;
	public int Hp;
	public int Sp;
	// Use this for initialization
	void Start() {
		Hp = 100;
		this.HpBar.GetComponent<TextMesh>().text = this.Hp.ToString();
		Sp = 0;
		this.SpBar.GetComponent<TextMesh>().text = this.Sp.ToString();
	}
	
	// Update is called once per frame
	void Update() {
	
	}

	public void IncreaseHp(int diff) {
		this.Hp += diff;
		this.HpBar.GetComponent<TextMesh>().text = this.Hp.ToString();
	}
	public void IncreaseSp(int diff) {
		this.Sp += diff;
		this.SpBar.GetComponent<TextMesh>().text = this.Sp.ToString();
	}
}
