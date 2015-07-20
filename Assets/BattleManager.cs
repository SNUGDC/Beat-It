using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour {
	public const float NETWORK_DELAY = 0.1f;

	public struct Data {
		public uint Id;
		public uint Judge;
		public Note.Button Button;
	}

	public Player[] Player;
	public JudgeDisplayer[] JudgeDisplayer;
	public Queue<BattleManager.Data>[] DataQueue;

	private int AttackerIndex;

	void Start () {
		this.AttackerIndex = 0;
		DataQueue = new Queue<BattleManager.Data>[2] {
			new Queue<BattleManager.Data>(),
			new Queue<BattleManager.Data>()
		};
	}
	
	void Update () {
	}

	public IEnumerator DoBattle(uint id, bool flip) {
		yield return new WaitForSeconds(BattleManager.NETWORK_DELAY);
		// dequeue one note from Dataqueue[0]
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
		// core battle logic
		Player Attacker = (this.AttackerIndex == 0) ? Player[0] : Player[1];
		Player Defender = (this.AttackerIndex == 0) ? Player[1] : Player[0];
		BattleManager.Data AttackData = (this.AttackerIndex == 0)
										? Player1Data
										: Player2Data;
		BattleManager.Data DefendData = (this.AttackerIndex == 0)
										? Player2Data
										: Player1Data;
		if(AttackData.Button != Note.Button.NONE) {
			if(AttackData.Button == DefendData.Button
			   && AttackData.Judge < DefendData.Judge) {
				Defender.IncreaseSp(10);
			}
			else {
				Defender.IncreaseHp(-10);
				Attacker.IncreaseSp(10);
			}
		}
		if(flip) FlipAttacker();
	}

	public void FlipAttacker() {
		if(this.AttackerIndex == 0) this.AttackerIndex = 1;
		else						this.AttackerIndex = 0;
	}
}