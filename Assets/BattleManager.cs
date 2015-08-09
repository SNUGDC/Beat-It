using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour {
	public const float NETWORK_DELAY = 0.05f;
	public const float FLIP_DELAY = 0.6f;

	public struct Data {
		public uint Id;
		public uint Judge;
		public Note.Button Button;
	}

	public Player[] Player;
	public JudgeDisplayer[] JudgeDisplayer;
	public Queue<BattleManager.Data>[] DataQueue;
	public Text ComboText;
	public Text BattleText; // TODO : remove this

	private Note.Button LastButton;
	private uint CurrentCombo;
	private int AttackerIndex;

	private void Start () {
		this.LastButton = Note.Button.NONE;
		this.CurrentCombo = 0;
		this.AttackerIndex = 0;
		Player[1].GetComponent<SpriteRenderer>().material.color
			= Color.white;
		Player[0].GetComponent<SpriteRenderer>().material.color
			= Color.red;
		DataQueue = new Queue<BattleManager.Data>[2] {
			new Queue<BattleManager.Data>(),
			new Queue<BattleManager.Data>()
		};
	}
	
	private void Update () {
	}

	public void GetReady(int playerIndex, Note.Button but) {
		Player targetPlayer = Player[playerIndex];
		if(playerIndex == AttackerIndex) {
			AttackSkill skill = targetPlayer.GetAttackSkill(but);
			uint combo = this.GetNextCombo(but,
										   this.LastButton,
										   skill.TurnLength);
			skill.PlayAnim(targetPlayer.Anim, combo);
		}
		else {
			DefendSkill skill = targetPlayer.GetDefendSkill(but);
			uint combo = 1;
			skill.PlayAnim(targetPlayer.Anim, combo);
		}
	}

	public IEnumerator DoBattle(uint id, bool flip) {
		yield return new WaitForSeconds(BattleManager.NETWORK_DELAY);
		
		BattleManager.Data Player1Data = GetData(0, id);
		BattleManager.Data Player2Data = GetData(1, id);

		// set judge text
		JudgeDisplayer[0].SetJudge(Player1Data.Judge / 10.0f,
								   Player1Data.Button);
		JudgeDisplayer[1].SetJudge(Player2Data.Judge / 10.0f,
								   Player2Data.Button);

		// assign basic variables
		Player Attacker = (this.AttackerIndex == 0) ? Player[0] : Player[1];
		Player Defender = (this.AttackerIndex == 0) ? Player[1] : Player[0];
		BattleManager.Data AttackData = (this.AttackerIndex == 0)
										? Player1Data
										: Player2Data;
		BattleManager.Data DefendData = (this.AttackerIndex == 0)
										? Player2Data
										: Player1Data;

		// calculate combo
		AttackSkill skill = Attacker.GetAttackSkill(AttackData.Button);
		this.CurrentCombo = this.GetNextCombo(
			AttackData.Button,
			this.LastButton,
			(skill != null) ? skill.TurnLength : 1
		);
		ComboText.text = CurrentCombo.ToString() + " Combo";

		// call BattleCore
		BattleCore(Attacker, AttackData, Defender, DefendData);

		// post-battle logic
		this.LastButton = AttackData.Button;
		if(flip) StartCoroutine(FlipAttacker());
	}

	// core battle logic
	private void BattleCore(Player attacker, BattleManager.Data attackData,
							Player defender, BattleManager.Data defendData) {
		AttackSkill attackerSkill = attacker.GetAttackSkill(attackData.Button);
		DefendSkill defenderSkill = defender.GetDefendSkill(defendData.Button);
		DefendSkill.DefendState defendResult;
		// if defender didn't press button, set result to HIT
		if(defendData.Button == Note.Button.NONE) {
			defendResult = DefendSkill.DefendState.HIT;
		}
		// if defender pressed button, but attacker didn't, set result to GUARD
		else if(attackData.Button == Note.Button.NONE) {
			defendResult = DefendSkill.DefendState.GUARD;
		}
		// both players pressed button
		else {
			defendResult
				= defenderSkill.DoDefend(attackerSkill.Name,
										 this.CurrentCombo,
										 attackData.Judge <= defendData.Judge);
		}
		// TODO : remove this if-else block
		// print result to BattleText
		if(attackData.Button == Note.Button.NONE
		   && defendData.Button == Note.Button.NONE) {
			BattleText.text = "NONE";
		}
		else	BattleText.text = defendResult.ToString();

		switch(defendResult) {
			case DefendSkill.DefendState.GUARD : {
				// defender Guards attack, and attacker keeps attack
				try {
					defender.Anim.SetTrigger("action");
					defender.DecreaseHp(attackerSkill.Damage[this.CurrentCombo - 1]
										* (1 - defenderSkill.DefendRate));
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
				} catch {}
				try {
					attacker.Anim.SetTrigger("hit");
					attacker.DecreaseHp(defenderSkill.Damage);
				} catch {}
				this.CurrentCombo = 0;
				break;
			}
			case DefendSkill.DefendState.HIT : {
				// attacker succeeded to hit defender
				try {
					defender.Anim.SetTrigger("hit");
					defender.DecreaseHp(attackerSkill.Damage[CurrentCombo - 1]);
				} catch {}
				if(attackData.Button != Note.Button.NONE){
					attacker.Anim.SetTrigger("action");
				}
				else{
					attacker.Anim.SetTrigger("hit");
				}
				break;
			}
			default : break;
		}
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

		yield return new WaitForSeconds(BattleManager.FLIP_DELAY);

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
	private BattleManager.Data GetData(int player, uint id) {
		if(DataQueue[player].Count != 0 && DataQueue[player].Peek().Id == id) {
			return DataQueue[player].Dequeue();
		}
		else {
			if(DataQueue[0].Count != 0) Debug.Log(DataQueue[0].Peek().Id);
			else Debug.Log(id + " Not Found!");
			return new BattleManager.Data {
				Id = id, Judge = 0, Button = Note.Button.NONE
			};
		}
	}

	// calculates next combo
	private uint GetNextCombo(Note.Button curBut, Note.Button prevBut,
							  uint turnLength) {
		if(curBut != Note.Button.NONE
		   && (curBut == prevBut
			   || prevBut == Note.Button.NONE)
		  ){
		  	return this.CurrentCombo % turnLength + 1;
		}
		else if(curBut != Note.Button.NONE) {
			return 1;
		}
		else {
			return 0;
		}
	}
}