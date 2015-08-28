using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour {
	public const float FLIP_DELAY = 0.6f;

	public GameObject[] WinImage;
	public Player[] Player;
	public Animator[] JudgeAnim;
	public Animator[] EffectAnim;
	public Queue<Note.Core>[] DataQueue;
	public Text ComboText;

	private Note.Button LastButton;
	private InputManager.InputType LastType;
	private int LongButtonTime;
	private uint CurrentCombo;
	[HideInInspector] public int AttackerIndex;
	private bool CancelFlip;

	void Start () {
		LastButton = Note.Button.NONE;
		LastType = InputManager.InputType.NONE;
		LongButtonTime = 0;
		CurrentCombo = 0;
		AttackerIndex = 0;
		CancelFlip = false;
		Player[0].GetComponent<SpriteRenderer>().material.color = Color.red;
		Player[1].GetComponent<SpriteRenderer>().material.color = Color.white;
		DataQueue = new Queue<Note.Core>[2] { new Queue<Note.Core>(), new Queue<Note.Core>() };
	}

	public void FlipAttackerOnStart() {
		AttackerIndex = 1;
		Player [0].GetComponent<SpriteRenderer> ().material.color = Color.white;
		Player [1].GetComponent<SpriteRenderer> ().material.color = Color.red;
	}

	void Update () {
	}

	public void PressUp(int playerIndex, Note.Button but) {
		Player targetPlayer = Player[playerIndex];
		AttackSkill skill = targetPlayer.GetAttackSkill(but);
		if(skill.IsLongButton && playerIndex == AttackerIndex
		   && but == LastButton && LastType != InputManager.InputType.UP) {
			targetPlayer.Anim.SetTrigger("hit");
			LastButton = Note.Button.NONE;
			LastType = InputManager.InputType.NONE;
		}
	}

	public bool IsValidInput(int playerIndex, Note.Button but, InputManager.InputType type) {
		Player targetPlayer = Player[playerIndex];
		if(playerIndex == AttackerIndex) {
			AttackSkill skill = targetPlayer.GetAttackSkill(but);
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
			targetPlayer.Anim.ResetTrigger("action");
			skill.PlayAnim(targetPlayer.Anim, combo, type == InputManager.InputType.UP);
		}
		else {
			DefendSkill skill = targetPlayer.GetDefendSkill(but);
			targetPlayer.Anim.ResetTrigger("action");
			skill.PlayAnim(targetPlayer.Anim, 1, false);
			if(skill.Name == "Guard") {
				EffectAnim[playerIndex].Play("guardmake", -1, 0);
			}
		}
	}

	public void ShowJudge(int playerIndex, uint judge) {
		if(judge >= 95) JudgeAnim[playerIndex].Play("amazing", -1, 0);
		else if(95 > judge && judge >= 80) JudgeAnim[playerIndex].Play("cool", -1, 0);
		else if(80 > judge && judge >= 65) JudgeAnim[playerIndex].Play("great", -1, 0);
		else if(65 > judge && judge >= 50) JudgeAnim[playerIndex].Play("good", -1, 0);
	}

	public void DoBattle(uint id, int time) {
		// assign basic variables
		Player attacker = (this.AttackerIndex == 0) ? Player[0] : Player[1];
		Player defender = (this.AttackerIndex == 0) ? Player[1] : Player[0];
		Note.Core attackData = (this.AttackerIndex == 0) ? GetData(0, id) : GetData(1, id);
		Note.Core defendData = (this.AttackerIndex == 0) ? GetData(1, id) : GetData(0, id);
		// show miss layer
		if(attackData.Judge < 50) JudgeAnim[0].Play("miss", -1, 0);
		if(defendData.Judge < 50) JudgeAnim[1].Play("miss", -1, 0);
		// calculate combo
		CurrentCombo = GetNextCombo(
			attackData.Button,
			attackData.Type,
			attacker.GetAttackSkill(attackData.Button)
		);
		ComboText.text = CurrentCombo.ToString() + " Combo";

		// call BattleCore
		BattleCore(attacker, attackData, defender, defendData, time);

		// post-battle logic
		this.LastButton = attackData.Button;
		this.LastType = attackData.Type;
		if(attacker.Hp.Value <= 0) {
			GameObject.Find("BeatGenerator").SetActive(false);
			GameObject.Find("InputManager1").SetActive(false);
			GameObject.Find("InputManager2").SetActive(false);
			attacker.Anim.speed = 0.5f;
			defender.Anim.speed = 0.5f;
			attacker.Anim.Play("lose");
			StartCoroutine(EndGame((AttackerIndex == 0) ? 1 : 0));
		}
		else if(defender.Hp.Value <= 0) {
			GameObject.Find("BeatGenerator").SetActive(false);
			GameObject.Find("InputManager1").SetActive(false);
			GameObject.Find("InputManager2").SetActive(false);
			attacker.Anim.speed = 0.5f;
			defender.Anim.speed = 0.5f;
			defender.Anim.Play("lose");
			StartCoroutine(EndGame(AttackerIndex));
		}
	}

	private IEnumerator EndGame(int winnerIndex) {
		yield return new WaitForSeconds(2.5f);
		Player[winnerIndex].Anim.speed = 1.0f;
		Player[winnerIndex].Anim.Play("win");
		yield return new WaitForSeconds(1);
		WinImage[winnerIndex].SetActive(true);
		yield return new WaitForSeconds(3.5f);
		Object.Destroy(GameObject.Find("GameData"));
		Application.LoadLevel("sceneSelect");
	}

	// core battle logic
	private void BattleCore(Player attacker, Note.Core attackData,
							Player defender, Note.Core defendData, int time) {
		AttackSkill attackerSkill = attacker.GetAttackSkill(attackData.Button);
		DefendSkill defenderSkill = defender.GetDefendSkill(defendData.Button);
		Animator attackerEffect = EffectAnim[AttackerIndex];
		Animator defenderEffect = EffectAnim[(AttackerIndex == 0) ? 1 : 0];
		DefendSkill.DefendState defendResult;
		// if both player didn't press button, set result to NONE
		if(defendData.Button == Note.Button.NONE && attackData.Button == Note.Button.NONE)
			defendResult = DefendSkill.DefendState.NONE;
		else if(defendData.Button == Note.Button.NONE
				&& attackData.Type == InputManager.InputType.DOWN
				&& attackerSkill.IsLongButton) {
			defendResult = DefendSkill.DefendState.GUARD;
		}
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
		switch(defendResult) {
			case DefendSkill.DefendState.GUARD : {
				// defender Guards attack, and attacker keeps attack
				try {
					if(defenderSkill.Name == "Guard") defenderEffect.Play("guardsuccess", -1, 0);
					defender.Anim.SetTrigger("action");
					defender.DecreaseHp(attackerSkill.Damage[this.CurrentCombo - 1]
										* (1 - defenderSkill.DefendRate));
					defender.IncreaseSp((int)defenderSkill.SkillPoint);
				} catch {}
				try {
					if(attackerSkill.IsLongButton && attackData.Type == InputManager.InputType.DOWN) {
						this.LongButtonTime = time;
					}
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
					attackerEffect.Play("cancel", -1, 0);
					attacker.Anim.Play("hit", -1, 0);
					attacker.DecreaseHp(defenderSkill.Damage);
				} catch {}
				this.CurrentCombo = 0;
				break;
			}
			case DefendSkill.DefendState.HIT : {
				// attacker succeeded to hit defender
				try {
					if(attackerSkill.IsLongButton && attackData.Type == InputManager.InputType.UP) {
						defender.Anim.Play("hit", -1, 0);
						float damage = (time - this.LongButtonTime) / 25000.0f;
						if (damage > 100)
							defenderEffect.Play("str_full", -1, 0);
						else
							defenderEffect.Play("str_nor", -1, 0);
						damage = System.Math.Min(damage, 100);
						defender.DecreaseHp(damage);
					}
					else if(attackerSkill.IsLongButton && attackData.Type == InputManager.InputType.KEEP) {
						// do nothing
					}
					else {
						if(attackerSkill.Name == "Consecutive" && CurrentCombo == 1)
							defenderEffect.Play("con1", -1, 0);
						else if(attackerSkill.Name == "Consecutive" && CurrentCombo == 2)
							defenderEffect.Play("con2", -1, 0);
						else if(attackerSkill.Name == "Consecutive" && CurrentCombo == 3)
							defenderEffect.Play("con3", -1, 0);
						else if(attackerSkill.Name == "Normal")
							defenderEffect.Play("kyunje", -1, 0);
						defender.Anim.Play("hit", -1, 0);
						defender.DecreaseHp(attackerSkill.Damage[CurrentCombo - 1]);
					}
				} catch {}
				attacker.Anim.SetTrigger("action");
				attacker.IncreaseSp((int)attackerSkill.SkillPoint[CurrentCombo - 1]);
				break;
			}
			case DefendSkill.DefendState.NONE : {
				// defender.Anim.Play("hit");
				// attacker.Anim.Play("hit");
				break;
			}
			default : break;
		}
		if(CurrentCombo == 3 && attackData.Button == Note.Button.BLUE
		   && defendResult == DefendSkill.DefendState.HIT)
			CancelFlip = true;
		if(attackData.Button == Note.Button.RED
		   && attackData.Type == InputManager.InputType.UP
		   && defendResult == DefendSkill.DefendState.HIT
		   && (time - this.LongButtonTime) / 25000.0f > 100)
			CancelFlip = true;
	}

	// flip attacking player
	public void FlipAttacker() {
		if(CancelFlip) {
			CancelFlip = false;
			return;
		}
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
				&& curType == InputManager.InputType.DOWN)
			return 1;
		// skill is long button & input is KEEP
		else if(skill.IsLongButton
				&& LastButton == curBut && LastType != InputManager.InputType.UP
				&& (curType == InputManager.InputType.KEEP
					|| curType == InputManager.InputType.UP))
			return 2;
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