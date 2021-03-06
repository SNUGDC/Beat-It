﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public delegate void PlayAnim(Animator target, uint combo, bool isUp);

	public Animator Anim; // Mechanim animator for this player
	public HpBar Hp; // Hp Text
	public SpBar Sp; // Sp Text
	private AttackSkill[] AttackSkillList;
	private DefendSkill[] DefendSkillList;
	private SpecialSkill[] SpecialSkillList;
	public int PlayerType; // 1P or 2P?

	void Start() {
		var gameData = GameObject.Find ("GameData").GetComponent<GameData>();
		AttackSkillList = new AttackSkill[3] {
			new AttackSkill(), new AttackSkill(), new AttackSkill()
		};
		DefendSkillList = new DefendSkill[3] {
			new DefendSkill(), new DefendSkill(), new DefendSkill()
		};
		SpecialSkillList = new SpecialSkill[2] {
			new SpecialSkill(gameData.characterData[PlayerType-1].character.skills[0]),
			new SpecialSkill(gameData.characterData[PlayerType-1].character.skills[1])
		};
		SetSkill("none"); // TODO : remove this
	}
	
	// Update is called once per frame
	void Update() {
	}

	public void SetSkill(string fileName) {
		// TODO : load from file
		AttackSkillList[0].Name = "Power";
		AttackSkillList[0].IsLongButton = true;
		AttackSkillList[0].TurnLength = 2;
		AttackSkillList[0].Damage = new uint[2] {0, 100};
		AttackSkillList[0].SkillPoint = new uint[2] {0, 0};
		AttackSkillList[0].PlayAnim
			= (Animator target, uint combo, bool isUp) => {
				if(combo == 1) target.Play("strong_ready_ready");
				else if(isUp)  target.Play("strong_success_ready");
			};
		AttackSkillList[1].Name = "Consecutive";
		AttackSkillList[1].IsLongButton = false;
		AttackSkillList[1].TurnLength = 3;
		AttackSkillList[1].Damage = new uint[3] {30, 40, 70};
		AttackSkillList[1].SkillPoint = new uint[3] {0, 0, 0};
		AttackSkillList[1].PlayAnim
			= (Animator target, uint combo, bool isUp) => {
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
		AttackSkillList[2].SkillPoint = new uint[1] {500};
		AttackSkillList[2].PlayAnim
			= (Animator target, uint combo, bool isUp) => {
				target.Play("normal_ready");
			};

		DefendSkillList[0].Name = "Guard";
		DefendSkillList[0].DefendRate = 0.5f;
		DefendSkillList[0].Damage = 0;
		DefendSkillList[0].SkillPoint = 0;
		DefendSkillList[0].DoDefend
			= (string attackSkill, bool defendableJudge, bool isUp) => {
				switch(attackSkill) {
					case "Power" :
						return (isUp)
							   ? DefendSkill.DefendState.HIT
							   : DefendSkill.DefendState.GUARD;
					case "Consecutive" :
						return DefendSkill.DefendState.GUARD;
					case "Normal" :
						return DefendSkill.DefendState.GUARD;
					default :
						return DefendSkill.DefendState.GUARD;
				}
			};
		DefendSkillList[0].PlayAnim
			= (Animator target, uint combo, bool isUp) => {
				target.Play("guard_ready");
			};
		DefendSkillList[1].Name = "Evade";
		DefendSkillList[1].DefendRate = 1.0f;
		DefendSkillList[1].Damage = 0;
		DefendSkillList[1].SkillPoint = 500;
		DefendSkillList[1].DoDefend
			= (string attackSkill, bool defendableJudge, bool isUp) => {
				switch(attackSkill) {
					case "Power" :
						return (!isUp || defendableJudge)
							   ? DefendSkill.DefendState.GUARD
							   : DefendSkill.DefendState.HIT;
					case "Consecutive" :
						return (defendableJudge)
							   ? DefendSkill.DefendState.GUARD
							   : DefendSkill.DefendState.HIT;
					case "Normal" :
						return DefendSkill.DefendState.HIT;
					default :
						return DefendSkill.DefendState.GUARD;
				}
			};
		DefendSkillList[1].PlayAnim
			= (Animator target, uint combo, bool isUp) => {
				target.Play("evade_ready");
			};
		DefendSkillList[2].Name = "Cancel";
		DefendSkillList[2].DefendRate = 1.0f;
		DefendSkillList[2].Damage = 20;
		DefendSkillList[2].SkillPoint = 500;
		DefendSkillList[2].DoDefend
			= (string attackSkill, bool defendableJudge, bool isUp) => {
				switch(attackSkill) {
					case "Power" :
						return (isUp)
							   ? DefendSkill.DefendState.HIT
							   : DefendSkill.DefendState.CANCEL;
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
			= (Animator target, uint combo, bool isUp) => {
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
			case Note.Button.GREEN	: return this.DefendSkillList[2];;
			default					: return null;
		}
	}

	public SpecialSkill GetSpecialSkill(Note.Button but) {
		switch (but) {
		case Note.Button.SKILL1:
			return this.SpecialSkillList [0];
		case Note.Button.SKILL2:
			return this.SpecialSkillList [1];
		default					:
			return null;
		}
	}

	public void DecreaseHp(int diff) {
		Hp.Increase(-diff);
	}

	// shortcut for float argument
	public void DecreaseHp(float diff) {
		this.DecreaseHp((int)diff);
	}

	public void IncreaseSp(int diff) {
		Sp.Increase(diff);
	}
}