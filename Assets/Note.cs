using UnityEngine;

public class Note{
	public enum Button {NONE, RED, BLUE, GREEN};

	public bool Flip; // flips attacker
	public bool IsValid; // is shown in display
	public int Time; // time of note

	private Button[] PressedButton; // accepted input for each player
	private InputManager.InputType[] PressedInput;
	private uint Id; // Id of Current Note
	private uint[] Judge; // judge of each player

	// default constructor
	public Note(uint id, int time) {
		Id = id;
		Time = time;
		Flip = false;
		IsValid = false;
		PressedButton = new Button[2] {Button.NONE, Button.NONE};
		PressedInput = new InputManager.InputType[2] {InputManager.InputType.NONE,
													  InputManager.InputType.NONE};
		Judge = new uint[2] {0, 0};
	}

	public bool IsPressed(int player) {
		return PressedButton[player] != Note.Button.NONE;
	}

	// create note on display
	public void Appear(Transform notePrefab) {
		Transform newTrans = Object.Instantiate(notePrefab,
												new Vector3(0, -19.72f, -1 + 0.001f * Id),
												Quaternion.identity) as Transform;
		IsValid = true;
		newTrans.parent = GameObject.Find("BeatGenerator").transform;
		newTrans.GetComponent<NoteMover>().NoteData = this;
	}

	// accept user input
	public bool Press(int player, int time, Note.Button but, InputManager.InputType type) {
		float judge = 1 - System.Math.Abs(time - this.Time)
						   / (float)(BeatGenerator.BEAT_MARGIN);
		// if timing is bad & UP signal is received
		if(judge <= 0 && type == InputManager.InputType.UP) {
			GameObject.Find("BattleManager").GetComponent<BattleManager>().PressUp(player, but);
			return false;	
		}
		// if timing is good & is valid input, accept
		else if(judge > 0 && GameObject.Find("BattleManager").GetComponent<BattleManager>()
															 .GetReady(player, but, type)){
			this.PressedButton[player] = but;
			this.PressedInput[player] = type;
			this.Judge[player] = (uint)(judge * 1000);
			return true;
		}
		else return false;
	}

	// kill note from display
	// calls actual battle logic
	public void Kill() {
		NetworkConnector network
			= GameObject.Find("NetworkManager").GetComponent<NetworkConnector>();
		BattleManager battleManager
			= GameObject.Find("BattleManager").GetComponent<BattleManager>();
		if(network.LocalPlayer[0]) {
			battleManager.DataQueue[0].Enqueue(new BattleManager.Data{
				Id = this.Id,
				Judge = this.Judge[0],
				Button = this.PressedButton[0],
				Type = this.PressedInput[0]
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
				Button = this.PressedButton[1],
				Type = this.PressedInput[1]
			});
		}
		else {
			network.SendString("1"
							   + ' ' + Id.ToString()
							   + ' ' + PressedButton[1].ToString()
							   + ' ' + this.Judge[1].ToString());
		}
		// call actual battle logic
		battleManager.StartCoroutine(battleManager.DoBattle(this.Id, this.Flip));
	}
}