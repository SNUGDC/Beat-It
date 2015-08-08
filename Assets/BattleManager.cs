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

	void Start () {
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
	
	void Update () {
	}

	public void GetReady(int playerIndex, Note.Button but) {
		Player targetPlayer = Player[playerIndex];
		if(playerIndex == AttackerIndex) {
			AttackSkill skill = targetPlayer.GetAttackSkill(but);
			uint combo = (this.CurrentCombo + 1) % skill.TurnLength + 1;
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
		// dequeue one note from DataQueue[0]
		BattleManager.Data Player1Data;
		if(DataQueue[0].Count != 0 && DataQueue[0].Peek().Id == id) {
			Player1Data = DataQueue[0].Dequeue();
		}
		else {
			Player1Data = new BattleManager.Data {
				Id = id, Judge = 0, Button = Note.Button.NONE
			};
			if(DataQueue[0].Count != 0) Debug.Log(DataQueue[0].Peek().Id);
			else Debug.Log(id + " Not Found!");
		}

		// dequeue one note from Dataqueue[1]
		BattleManager.Data Player2Data;
		if(DataQueue[1].Count != 0 && DataQueue[1].Peek().Id == id) {
			Player2Data = DataQueue[1].Dequeue();
		}
		else {
			Player2Data = new BattleManager.Data {
				Id = id, Judge = 0, Button = Note.Button.NONE
			};
			if(DataQueue[1].Count != 0) Debug.Log(DataQueue[1].Peek().Id);
			else Debug.Log(id + " Not Found!");
		}

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
		if(AttackData.Button != Note.Button.NONE
		   && (AttackData.Button == this.LastButton
			   || this.LastButton == Note.Button.NONE)
		  ){
			CurrentCombo++;
			CurrentCombo %= Attacker.GetAttackSkill(AttackData.Button)
									.TurnLength;
		}
		else {
			CurrentCombo = 0;
		}
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
		if(defendData.Button == Note.Button.NONE) {
			defendResult = DefendSkill.DefendState.HIT;
		}
		else if(attackData.Button == Note.Button.NONE) {
			defendResult = DefendSkill.DefendState.GUARD;
		}
		else {
			defendResult
				= defenderSkill.DoDefend(attackerSkill.Name,
										 this.CurrentCombo,
										 attackData.Judge < defendData.Judge);
		}
		if(attackData.Button == Note.Button.NONE
		   && defendData.Button == Note.Button.NONE) {
			BattleText.text = "";
		}
		else	BattleText.text = defendResult.ToString();
		switch(defendResult) {
			case DefendSkill.DefendState.GUARD : {
				try {
					defender.Anim.SetTrigger("action");
					defender.DecreaseHp(attackerSkill.Damage[this.CurrentCombo]
										* (1 - defenderSkill.DefendRate));
				} catch {}
				try {
					attacker.Anim.SetTrigger("action");
					attacker.DecreaseHp(defenderSkill.Damage);
				} catch {}
				break;
			}
			case DefendSkill.DefendState.CANCEL : {
				defender.Anim.SetTrigger("action");
				defender.DecreaseHp(attackerSkill.Damage[this.CurrentCombo]
									* (1 - defenderSkill.DefendRate));
				attacker.Anim.SetTrigger("hit");
				attacker.DecreaseHp(defenderSkill.Damage);
				this.CurrentCombo = 0;
				break;
			}
			case DefendSkill.DefendState.HIT : {
				try {
					defender.Anim.SetTrigger("hit");
					defender.DecreaseHp(attackerSkill.Damage[CurrentCombo]);
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

	private IEnumerator FlipAttacker() {
		foreach(AnimatorControllerParameter param
				in Player[0].Anim.parameters) {
			Player[0].Anim.ResetTrigger(param.name);
		}
		foreach(AnimatorControllerParameter param
				in Player[1].Anim.parameters) {
			Player[1].Anim.ResetTrigger(param.name);
		}
		Player[0].Anim.SetTrigger("reset");
		Player[1].Anim.SetTrigger("reset");
		yield return new WaitForSeconds(BattleManager.NETWORK_DELAY);

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
	}
}