using UnityEngine;
using System.Collections;
using System.Linq;

public class InputManager : MonoBehaviour {
	public const int KEEP_MARGIN = 10000;
	public enum InputType {NONE, DOWN, KEEP, UP};

	public BeatGenerator BeatGen;
	public BattleManager BattleMan;
	public int Player;

	private InputType[] CurInput;
	private bool[] IsLongPressed;

	// interpret input & put data into queue
	public void OnSocketRead(string input) {
		// message format is
		// "player noteId judge button"
		string[] splitInput = input.Split(' ');
		int player = System.Convert.ToInt32(splitInput[0]);
		BattleManager.Data newData = new BattleManager.Data {
			Id = System.Convert.ToUInt32(splitInput[1]),
			Judge = System.Convert.ToUInt32(splitInput[2]),
			Button = (Note.Button)System.Enum.Parse(typeof(Note.Button), splitInput[3])
		};
		// add interpreted data into data queue
		BattleMan.DataQueue[player].Enqueue(newData);
	}

	private KeyCode ButToKeycode(Note.Button but) {
		switch(but) {
			case Note.Button.RED :
				return (Player == 0) ? KeyCode.A : KeyCode.J;
			case Note.Button.BLUE :
				return (Player == 0) ? KeyCode.S : KeyCode.K;
			case Note.Button.GREEN :
				return (Player == 0) ? KeyCode.D : KeyCode.L;
			default :
				return KeyCode.None;
		}
	}

	// Use this for initialization
	void Start () {
		IsLongPressed = new bool[3] {true, true, true};
		CurInput = new InputManager.InputType[3];
		GameObject.Find("NetworkManager").GetComponent<NetworkConnector>()
			.OnRead = this.OnSocketRead;
	}

	// Update is called once per frame
	void Update () {
		int curTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000);
		UpdateInput();
		// find first unpressed note in VALID State for player
		Note target = BeatGen.NoteList.First(
			n => n.IsValid && !n.IsPressed(Player)
		);
		// return if no target note found
		if(target == null) return;
		// if button is up or down, send event
		if(CurInput[0] == InputManager.InputType.DOWN
		   || CurInput[0] == InputManager.InputType.UP)
			target.Press(Player, curTime - BeatGen.StartTime, Note.Button.RED, CurInput[0]);
		else if(CurInput[1] == InputManager.InputType.DOWN
				|| CurInput[1] == InputManager.InputType.UP)
			target.Press(Player, curTime - BeatGen.StartTime, Note.Button.BLUE, CurInput[1]);
		else if(CurInput[2] == InputManager.InputType.DOWN
				|| CurInput[2] == InputManager.InputType.UP)
			target.Press(Player, curTime - BeatGen.StartTime, Note.Button.GREEN, CurInput[2]);
		// if button is keep at appropriate time, send event
		else if(System.Math.Abs(target.Time - curTime) <= InputManager.KEEP_MARGIN) {
			if(CurInput[0] == InputManager.InputType.KEEP)
				target.Press(Player, curTime - BeatGen.StartTime, Note.Button.RED, CurInput[0]);
			else if(CurInput[1] == InputManager.InputType.KEEP)
				target.Press(Player, curTime - BeatGen.StartTime, Note.Button.RED, CurInput[1]);
			else if(CurInput[2] == InputManager.InputType.KEEP)
				target.Press(Player, curTime - BeatGen.StartTime, Note.Button.RED, CurInput[2]);
		}
		// if button is accepted, reset long button detector
		if(target.IsPressed(Player)) ResetLongButton();
	}

	private void ResetLongButton() {
		IsLongPressed[0] = IsLongPressed[1] = IsLongPressed[2] = true;
	}

	private void UpdateInput() {
		// update IsLongPressed
		if(CurInput[0] == InputManager.InputType.UP) IsLongPressed[0] = false;
		if(CurInput[1] == InputManager.InputType.UP) IsLongPressed[1] = false;
		if(CurInput[2] == InputManager.InputType.UP) IsLongPressed[2] = false;
		// process red button
		if(Input.GetKeyDown(ButToKeycode(Note.Button.RED)))
			CurInput[0] = InputManager.InputType.DOWN;
		else if(Input.GetKeyUp(ButToKeycode(Note.Button.RED))) {
			CurInput[0] = (IsLongPressed[0])
						  ? InputManager.InputType.UP
						  : InputManager.InputType.NONE;
		}
		else if(Input.GetKey(ButToKeycode(Note.Button.RED)))
			CurInput[0] = (IsLongPressed[0])
						  ? InputManager.InputType.KEEP
						  : InputManager.InputType.NONE;
		else
			CurInput[0] = InputManager.InputType.NONE;
		// process blue button
		if(Input.GetKeyDown(ButToKeycode(Note.Button.BLUE)))
			CurInput[1] = InputManager.InputType.DOWN;
		else if(Input.GetKeyUp(ButToKeycode(Note.Button.BLUE))){
			CurInput[1] = (IsLongPressed[1])
						  ? InputManager.InputType.UP
						  : InputManager.InputType.NONE;
		}
		else if(Input.GetKey(ButToKeycode(Note.Button.BLUE)))
			CurInput[1] = (IsLongPressed[1])
						  ? InputManager.InputType.KEEP
						  : InputManager.InputType.NONE;
		else
			CurInput[1] = InputManager.InputType.NONE;
		// process green button
		if(Input.GetKeyDown(ButToKeycode(Note.Button.GREEN)))
			CurInput[2] = InputManager.InputType.DOWN;
		else if(Input.GetKeyUp(ButToKeycode(Note.Button.GREEN))){
			CurInput[2] = (IsLongPressed[2])
						  ? InputManager.InputType.UP
						  : InputManager.InputType.NONE;
		}
		else if(Input.GetKey(ButToKeycode(Note.Button.GREEN)))
			CurInput[2] = (IsLongPressed[2])
						  ? InputManager.InputType.KEEP
						  : InputManager.InputType.NONE;
		else
			CurInput[2] = InputManager.InputType.NONE;
	}
}