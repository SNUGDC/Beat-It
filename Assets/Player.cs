using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public delegate void PlayAnim(Animator target, uint combo);
	public Animator Anim; // Mechanim animator for this player
	public TextMesh HpBar; // Hp Text
	public TextMesh SpBar; // Sp Text
	private int Hp;
	private int Sp;
	private AttackSkill[] AttackSkillList;
	private DefendSkill[] DefendSkillList;

	void Start() {
		Hp = 100;
		this.HpBar.text = this.Hp.ToString();
		Sp = 0;
		this.SpBar.text = this.Sp.ToString();
		AttackSkillList = new AttackSkill[3] {
			new AttackSkill(), new AttackSkill(), new AttackSkill()
		};
		DefendSkillList = new DefendSkill[3] {
			new DefendSkill(), new DefendSkill(), new DefendSkill()
		};
		SetSkill("none"); // TODO : remove this
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
		AttackSkillList[0].PlayAnim
			= (Animator target, uint combo) => {
				if(combo == 1) target.Play("strong_ready_ready");
				else		   target.Play("strong_success_ready");
			};
		AttackSkillList[1].Name = "Consecutive";
		AttackSkillList[1].IsLongButton = false;
		AttackSkillList[1].TurnLength = 3;
		AttackSkillList[1].Damage = new uint[3] {30, 40, 70};
		AttackSkillList[1].PlayAnim
			= (Animator target, uint combo) => {
				switch(combo) {
					case 1 : target.Play("consecutive1_ready"); break;
					case 2 : target.Play("consecutive2_ready"); break;
					default : target.Play("consecutive3_ready"); break;
				}
			};
		AttackSkillList[2].Name = "Normal";
		AttackSkillList[2].IsLongButton = false;
		AttackSkillList[2].TurnLength = 1;
		AttackSkillList[2].Damage = new uint[1] {20};
		AttackSkillList[2].PlayAnim
			= (Animator target, uint combo) => {
				target.Play("normal_ready");
			};

		DefendSkillList[0].Name = "Guard";
		DefendSkillList[0].DefendRate = 0.5f;
		DefendSkillList[0].Damage = 0;
		DefendSkillList[0].DoDefend
			= (string attackSkill, uint turn, bool defendableJudge) => {
				switch(attackSkill) {
					case "Power" :
						return (turn == 1)
							   ? DefendSkill.DefendState.GUARD
							   : DefendSkill.DefendState.HIT;
					case "Consecutive" :
						return DefendSkill.DefendState.GUARD;
					case "Normal" :
						return DefendSkill.DefendState.GUARD;
					default :
						return DefendSkill.DefendState.GUARD;
				}
			};
		DefendSkillList[0].PlayAnim
			= (Animator target, uint combo) => {
				target.Play("guard_ready");
			};
		DefendSkillList[1].Name = "Evade";
		DefendSkillList[1].DefendRate = 1.0f;
		DefendSkillList[1].Damage = 0;
		DefendSkillList[1].DoDefend
			= (string attackSkill, uint turn, bool defendableJudge) => {
				switch(attackSkill) {
					case "Power" :
						return (turn == 1 || defendableJudge)
							   ? DefendSkill.DefendState.GUARD
							   : DefendSkill.DefendState.HIT;
					case "Consecutive" :
						return (defendableJudge)
							   ? DefendSkill.DefendState.CANCEL
							   : DefendSkill.DefendState.HIT;
					case "Normal" :
						return DefendSkill.DefendState.HIT;
					default :
						return DefendSkill.DefendState.GUARD;
				}
			};
		DefendSkillList[1].PlayAnim
			= (Animator target, uint combo) => {
				target.Play("evade_ready");
			};
		DefendSkillList[2].Name = "Cancel";
		DefendSkillList[2].DefendRate = 1.0f;
		DefendSkillList[2].Damage = 20;
		DefendSkillList[2].DoDefend
			= (string attackSkill, uint turn, bool defendableJudge) => {
				switch(attackSkill) {
					case "Power" :
						return (turn == 1)
							   ? DefendSkill.DefendState.CANCEL
							   : DefendSkill.DefendState.HIT;
					case "Consecutive" :
						return DefendSkill.DefendState.HIT;
					case "Normal" :
						return (defendableJudge)
							   ? DefendSkill.DefendState.CANCEL
							   : DefendSkill.DefendState.HIT;
					default :
						return DefendSkill.DefendState.GUARD;
				}
			};
		DefendSkillList[2].PlayAnim
			= (Animator target, uint combo) => {
				target.Play("cancel_ready");
			};
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

	public void DecreaseHp(int diff) {
		this.Hp -= diff;
		this.HpBar.text = this.Hp.ToString();
	}

	// shortcut for float argument
	public void DecreaseHp(float diff) {
		this.DecreaseHp((int)diff);
	}

	public void IncreaseSp(int diff) {
		this.Sp += diff;
		this.SpBar.text = this.Sp.ToString();
	}
}
