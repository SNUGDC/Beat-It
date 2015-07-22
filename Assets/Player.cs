using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public TextMesh HpBar;
	public TextMesh SpBar;
	public int Hp;
	public int Sp;
	// Use this for initialization
	void Start() {
		Hp = 100;
		this.HpBar.text = this.Hp.ToString();
		Sp = 0;
		this.SpBar.text = this.Sp.ToString();
	}
	
	// Update is called once per frame
	void Update() {
	
	}

	public void IncreaseHp(int diff) {
		this.Hp += diff;
		this.HpBar.text = this.Hp.ToString();
	}
	public void IncreaseSp(int diff) {
		this.Sp += diff;
		this.SpBar.text = this.Sp.ToString();
	}
}
