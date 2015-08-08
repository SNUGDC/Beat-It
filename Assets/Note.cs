using UnityEngine;

public class Note{
	public enum Button {NONE, RED, BLUE, GREEN};
	
	public const uint TIME_MARGIN = 500000; // time margin for judge
	// key table for each button
	public static readonly KeyCode[,] ButtonTable
		= {{KeyCode.None, KeyCode.A, KeyCode.S, KeyCode.D},
		   {KeyCode.None, KeyCode.J, KeyCode.K, KeyCode.L}};

	private uint Id; // Id of Current Note
	private bool Flip; // flips attacker
	private uint[] Judge; // judge of each player

	public bool IsValid; // is shown in display
	public int Time; // time of note
	public Button[] PressedButton; // accepted input for each player

	// default constructor
	public Note(uint id, int time) {
		Id = id;
		Time = time;
		Flip = false;
		IsValid = false;
		PressedButton = new Button[2] {Button.NONE, Button.NONE};
		Judge = new uint[2] {0, 0};
	}

	// create note on display
	public void Appear(BeatGenerator generator, Transform notePrefab) {
		Transform newTrans
			= Object.Instantiate(notePrefab,
								 new Vector3(0, -19.72f, -1 + 0.001f * Id),
								 Quaternion.identity) as Transform;
		IsValid = true;
		newTrans.parent = generator.transform;
		newTrans.GetComponent<NoteMover>().NoteData = this;
	}

	// accept user input
	public void Press(int player, int time, Button button) {
		if(System.Math.Abs(time - this.Time) < TIME_MARGIN){
			// set button pressed
			this.PressedButton[player] = button;
			// set judge
			float tempJudge = 1 - System.Math.Abs(time - this.Time)
								  / (float)(TIME_MARGIN);
			this.Judge[player] = (uint)(tempJudge * 1000);
			GameObject.Find("BattleManager").GetComponent<BattleManager>()
				.GetReady(player, button);
		}
	}

	// kill note from display
	// calls actual battle logic
	public void Kill() {
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
		// call actual battle logic
		battleManager.StartCoroutine(battleManager.DoBattle(this.Id,
															this.Flip));
	}
}