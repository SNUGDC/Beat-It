using UnityEngine;

public class Note{
	public enum Button {NONE, RED, BLUE, GREEN};
	
	public const uint TIME_MARGIN = 500000;
	public static readonly KeyCode[,] ButtonTable
		= {{KeyCode.None, KeyCode.A, KeyCode.S, KeyCode.D},
		   {KeyCode.None, KeyCode.J, KeyCode.K, KeyCode.L}};

	public uint Id; // Id starts from 1
					// Id number 0 refers to flipping attacker
	public bool Flip; // flips attacker
	public bool IsValid;
	public Button[] PressedButton;
	public uint[] Judge;
	public int Time;

	public Note(uint id, int time = 0) {
		Id = id;
		Flip = false;
		IsValid = false;
		PressedButton = new Button[2] {Button.NONE, Button.NONE};
		Judge = new uint[2] {0, 0};
		Time = time;
	}

	public void Appear(BeatGenerator generator, Transform notePrefab) {
		Transform newTrans
			= Object.Instantiate(notePrefab,
								 new Vector3(0, -19.72f, -1 + 0.001f * Id),
								 Quaternion.identity) as Transform;
		IsValid = true;
		newTrans.parent = generator.transform;
		newTrans.GetComponent<NoteMover>().NoteData = this;
	}

	public void Press(int player, int time, Button button) {
		if(System.Math.Abs(time - this.Time) < TIME_MARGIN){
			// set button pressed
			this.PressedButton[player] = button;
			// set judge
			float tempJudge = 1 - System.Math.Abs(time - this.Time)
								  / (float)(TIME_MARGIN);
			this.Judge[player] = (uint)(tempJudge * 1000);
		}
	}

	public void Kill(BeatGenerator generator, NoteMover mover) {
		Object.Destroy(mover.gameObject);
		generator.NoteList.Dequeue();
		NetworkConnector network = GameObject.Find("NetworkManager")
											 .GetComponent<NetworkConnector>();
		BattleManager battleManager = GameObject.Find("BattleManager")
												.GetComponent<BattleManager>();
		if(network.LocalPlayer[0]) {
			battleManager.DataQueue[0].Enqueue(new BattleManager.Data{
					Id = this.Id,
					Judge = this.Judge[0],
					Button = this.PressedButton[0]
				});
		}
		else {
			network.SendString("0"
							   + ' ' + Id.ToString()
							   + ' ' + PressedButton[0].ToString()
							   + ' ' + this.Judge[0].ToString());
		}
		if(network.LocalPlayer[1]) {
			battleManager.DataQueue[1].Enqueue(new BattleManager.Data{
					Id = this.Id,
					Judge = this.Judge[1],
					Button = this.PressedButton[1]
				});
		}
		else {
			network.SendString("1"
							   + ' ' + Id.ToString()
							   + ' ' + PressedButton[1].ToString()
							   + ' ' + this.Judge[1].ToString());
		}
		// wait call DoBattle after 100ms
		battleManager.StartCoroutine(battleManager.DoBattle(this.Id,
															this.Flip));
	}
}