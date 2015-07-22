using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public TextMesh HpBar;
	public TextMesh SpBar;
	public int Hp;
	public int Sp;
	private AttackSkill[] AttackSkillList;
	private DefendSkill[] DefendSkillList;
	// Use this for initialization
	void Start() {
		Hp = 100;
		this.HpBar.text = this.Hp.ToString();
		Sp = 0;
		this.SpBar.text = this.Sp.ToString();
		SetSkill("none"); // TODO : remove this
		AttackSkillList = new AttackSkill[3];
		DefendSkillList = new DefendSkill[3];
	}
	
	// Update is called once per frame
	void Update() {
	
	}

	public void SetSkill(string fileName) {
		// TODO : load from file
		AttackSkillList[0].Name = "Power";
		AttackSkillList[0].IsLongButton = false; // TODO : change to true
		AttackSkillList[0].TurnLength = 2;
		AttackSkillList[0].Damage = new uint[2] {0, 100};
		AttackSkillList[1].Name = "Consecutive";
		AttackSkillList[1].IsLongButton = false;
		AttackSkillList[1].TurnLength = 3;
		AttackSkillList[1].Damage = new uint[3] {30, 40, 70};
		AttackSkillList[2].Name = "Normal";
		AttackSkillList[2].IsLongButton = false;
		AttackSkillList[2].TurnLength = 1;
		AttackSkillList[2].Damage = new uint[1] {20};
	}

	public AttackSkill GetAttackSkill(Note.Button but) {
		switch(but) {
			case Note.Button.RED	: return this.AttackSkillList[0];
			case Note.Button.BLUE	: return this.AttackSkillList[1];
			case Note.Button.GREEN	: return this.AttackSkillList[2];
			default					: return null;
		}
	}

	public DefendSkill GetDefendSkill(Note.Button but) {
		switch(but) {
			case Note.Button.RED	: return this.DefendSkillList[0];
			case Note.Button.BLUE	: return this.DefendSkillList[1];
			case Note.Button.GREEN	: return this.DefendSkillList[2];
			default					: return null;
		}
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
