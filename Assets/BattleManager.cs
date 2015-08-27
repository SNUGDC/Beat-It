using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour {
	public const float FLIP_DELAY = 0.6f;

	public Player[] Player;
	public Animator[] JudgeAnim;
	public Queue<Note.Core>[] DataQueue;
	public Text ComboText;

	private Note.Button LastButton;
	private InputManager.InputType LastType;
	private uint CurrentCombo;
	private int AttackerIndex;
	private bool CancelFlip;

	void Start () {
		LastButton = Note.Button.NONE;
		LastType = InputManager.InputType.NONE;
		CurrentCombo = 0;
		AttackerIndex = 0;
		CancelFlip = false;
		Player[0].GetComponent<SpriteRenderer>().material.color = Color.red;
		Player[1].GetComponent<SpriteRenderer>().material.color = Color.white;
		DataQueue = new Queue<Note.Core>[2] { new Queue<Note.Core>(), new Queue<Note.Core>() };
	}

	void Update () {
	}

	public void PressUp(int playerIndex, Note.Button but) {
		Player targetPlayer = Player[playerIndex];
		AttackSkill skill = targetPlayer.GetAttackSkill(but);
		if(skill.IsLongButton && playerIndex == AttackerIndex
		   && but == LastButton && LastType != InputManager.InputType.UP) {
			targetPlayer.Anim.SetTrigger("hit");
			this.LastButton = Note.Button.NONE;
			this.LastType = InputManager.InputType.NONE;
		}
	}

	public bool IsValidInput(int playerIndex, Note.Button but, InputManager.InputType type) {
		Player targetPlayer = Player[playerIndex];
		if(playerIndex == AttackerIndex) {
			AttackSkill skill = targetPlayer.GetAttackSkill(but);
			uint combo = this.GetNextCombo(but, type, skill);
			// skill is not long button -> ignore long button input
			if(!skill.IsLongButton
			   && (type == InputManager.InputType.KEEP || type == InputManager.InputType.UP)) {
				return false;
			}
			// UP or KEEP signal came without DOWN signal -> ignore input
			if(LastButton != but
			   && (type == InputManager.InputType.KEEP || type == InputManager.InputType.UP)) {
				return false;
			}
			// long button is pressed too long -> ignore input & wait for UP signal
			if(skill.IsLongButton && combo >= skill.TurnLength
			   && type == InputManager.InputType.KEEP) {
				return false;
			}
			return true;
		}
		else {
			// ignore long button input
			if(type == InputManager.InputType.KEEP || type == InputManager.InputType.UP) {
				return false;
			}
			return true;
		}
	}

	public void GetReady(int playerIndex, Note.Button but, InputManager.InputType type) {
		Player targetPlayer = Player[playerIndex];
		if(playerIndex == AttackerIndex) {
			AttackSkill skill = targetPlayer.GetAttackSkill(but);
			uint combo = this.GetNextCombo(but, type, skill);
			// skill is not long button -> ignore long button input
			skill.PlayAnim(targetPlayer.Anim, combo, type == InputManager.InputType.UP);
		}
		else {
			DefendSkill skill = targetPlayer.GetDefendSkill(but);
			uint combo = 1;
			skill.PlayAnim(targetPlayer.Anim, combo, false);
		}
	}

	public void ShowJudge(int playerIndex, uint judge) {
		if(judge >= 95) JudgeAnim[playerIndex].Play("amazing");
		else if(95 > judge && judge >= 80) JudgeAnim[playerIndex].Play("cool");
		else if(80 > judge && judge >= 65) JudgeAnim[playerIndex].Play("great");
		else if(65 > judge && judge >= 50) JudgeAnim[playerIndex].Play("good");
	}

	public void DoBattle(uint id, bool flip) {
		
		Note.Core Player1Data = GetData(0, id);
		Note.Core Player2Data = GetData(1, id);

		if(Player1Data.Judge < 50) JudgeAnim[0].Play("miss");
		if(Player2Data.Judge < 50) JudgeAnim[1].Play("miss");

		// assign basic variables
		Player Attacker = (this.AttackerIndex == 0) ? Player[0] : Player[1];
		Player Defender = (this.AttackerIndex == 0) ? Player[1] : Player[0];

		Note.Core AttackData = (this.AttackerIndex == 0) ? Player1Data : Player2Data;
		Note.Core DefendData = (this.AttackerIndex == 0) ? Player2Data : Player1Data;

		// calculate combo
		AttackSkill skill = Attacker.GetAttackSkill(AttackData.Button);
		this.CurrentCombo = this.GetNextCombo(
			AttackData.Button,
			AttackData.Type,
			skill
		);
		ComboText.text = CurrentCombo.ToString() + " Combo";

		// call BattleCore
		BattleCore(Attacker, AttackData, Defender, DefendData);

		// post-battle logic
		this.LastButton = AttackData.Button;
		this.LastType = AttackData.Type;
		if(flip) {
			if(!CancelFlip) StartCoroutine(FlipAttacker());
			else CancelFlip = false;
		}
	}

	// core battle logic
	private void BattleCore(Player attacker, Note.Core attackData,
							Player defender, Note.Core defendData) {
		AttackSkill attackerSkill = attacker.GetAttackSkill(attackData.Button);
		DefendSkill defenderSkill = defender.GetDefendSkill(defendData.Button);
		DefendSkill.DefendState defendResult;
		// if both player didn't press button, set result to NONE
		if(defendData.Button == Note.Button.NONE && attackData.Button == Note.Button.NONE)
			defendResult = DefendSkill.DefendState.NONE;
		// if defender didn't press button, set result to HIT
		else if(defendData.Button == Note.Button.NONE) {
			defendResult = DefendSkill.DefendState.HIT;
		}
		// if attacker didn't press button, set result to GUARD
		else if(attackData.Button == Note.Button.NONE) {
			defendResult = DefendSkill.DefendState.GUARD;
		}
		// both players pressed button
		else {
			defendResult = defenderSkill.DoDefend(attackerSkill.Name,
												  attackData.Judge <= defendData.Judge,
												  attackData.Type == InputManager.InputType.UP);
		}
		attacker.Anim.ResetTrigger("action");
		defender.Anim.ResetTrigger("action");
		switch(defendResult) {
			case DefendSkill.DefendState.GUARD : {
				// defender Guards attack, and attacker keeps attack
				try {
					defender.Anim.SetTrigger("action");
					defender.DecreaseHp(attackerSkill.Damage[this.CurrentCombo - 1]
										* (1 - defenderSkill.DefendRate));
					defender.IncreaseSp((int)defenderSkill.SkillPoint);
				} catch {}
				try {
					attacker.Anim.SetTrigger("action");
					attacker.DecreaseHp(defenderSkill.Damage);
				} catch {}
				break;
			}
			case DefendSkill.DefendState.CANCEL : {
				// defender Guards attack, and attacker's combo is reset
				try {
					defender.Anim.SetTrigger("action");
					defender.DecreaseHp(attackerSkill.Damage[this.CurrentCombo - 1]
										* (1 - defenderSkill.DefendRate));
					defender.IncreaseSp((int)defenderSkill.SkillPoint);
				} catch {}
				try {
					attacker.Anim.Play("hit");
					attacker.DecreaseHp(defenderSkill.Damage);
				} catch {}
				this.CurrentCombo = 0;
				break;
			}
			case DefendSkill.DefendState.HIT : {
				// attacker succeeded to hit defender
				try {
					defender.Anim.Play("hit");
					defender.DecreaseHp(attackerSkill.Damage[CurrentCombo - 1]);
				} catch {}
				attacker.Anim.SetTrigger("action");
				attacker.IncreaseSp((int)attackerSkill.SkillPoint[CurrentCombo - 1]);
				break;
			}
			case DefendSkill.DefendState.NONE : {
				defender.Anim.Play("hit");
				attacker.Anim.Play("hit");
				break;
			}
			default : break;
		}
		if(CurrentCombo == 3 && attackData.Button == Note.Button.BLUE
		   && defendResult == DefendSkill.DefendState.HIT)
			CancelFlip = true;
		if(CurrentCombo == 2 && attackData.Button == Note.Button.RED
		   && defendResult == DefendSkill.DefendState.HIT)
			CancelFlip = true;
	}

	// flip attacking player
	private IEnumerator FlipAttacker() {
		// flip attacker sign & reset combo
		if(this.AttackerIndex == 0) {
			this.AttackerIndex = 1;
			Player[0].GetComponent<SpriteRenderer>().material.color
				= Color.white;
			Player[1].GetComponent<SpriteRenderer>().material.color
				= Color.red;
		}
		else {
			this.AttackerIndex = 0;
			Player[1].GetComponent<SpriteRenderer>().material.color
				= Color.white;
			Player[0].GetComponent<SpriteRenderer>().material.color
				= Color.red;
		}
		this.CurrentCombo = 0;
		this.LastButton = Note.Button.NONE;
		this.LastType = InputManager.InputType.NONE;

		yield return new WaitForSeconds(FLIP_DELAY);

		// reset all triggers to avoid unwanted animation
		foreach(AnimatorControllerParameter param
				in Player[0].Anim.parameters) {
			Player[0].Anim.ResetTrigger(param.name);
		}
		foreach(AnimatorControllerParameter param
				in Player[1].Anim.parameters) {
			Player[1].Anim.ResetTrigger(param.name);
		}
		// force playing basic animation
		Player[0].Anim.Play("basic");
		Player[1].Anim.Play("basic");
	}

	// dequeue one note from DataQueue
	// if no data exists or id dismatches, return initialized data
	private Note.Core GetData(int player, uint id) {
		if(DataQueue[player].Count != 0 && DataQueue[player].Peek().Id == id) {
			return DataQueue[player].Dequeue();
		}
		else {
			if(DataQueue[0].Count != 0) Debug.Log(DataQueue[0].Peek().Id);
			else Debug.Log(id + " Not Found!");
			return new Note.Core {
				Id = id, Button = Note.Button.NONE, Type = InputManager.InputType.NONE, Judge = 0
			};
		}
	}

	// calculates next combo
	private uint GetNextCombo(Note.Button curBut,
							  InputManager.InputType curType,
							  AttackSkill skill) {
		// skill not found
		if(skill == null)
			return 0;
		// skill is long button & DOWN signal received twice
		else if(skill.IsLongButton
				&& curType == InputManager.InputType.DOWN
				&& LastType == InputManager.InputType.DOWN)
			return 1;
		// skill is long button & released at last turn
		else if(skill.IsLongButton && curBut == LastButton && LastType == InputManager.InputType.UP)
			return 1;
		// new button pressed
		else if(curBut != Note.Button.NONE && LastButton == Note.Button.NONE)
			return 1;
		// button accepted -> combo increase
		else if(curBut != Note.Button.NONE && curBut == LastButton)
			return this.CurrentCombo % skill.TurnLength + 1;
		else
			return 1;
	}
}