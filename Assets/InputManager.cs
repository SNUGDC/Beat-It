using UnityEngine;
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
	private void OnSocketRead(string input) {
		// message format is
		// "player noteId button type judge"
		string[] splitInput = input.Split(' ');
		int player = System.Convert.ToInt32(splitInput[0]);
		Note.Core newData = new Note.Core {
			Id = System.Convert.ToUInt32(splitInput[1]),
			Button = (Note.Button)System.Enum.Parse(typeof(Note.Button), splitInput[2]),
			Type = (InputType)System.Enum.Parse(typeof(InputType), splitInput[3]),
			Judge = System.Convert.ToUInt32(splitInput[4])
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
			case Note.Button.SKILL :
				return (Player == 0) ? KeyCode.W : KeyCode.I;
			default :
				return KeyCode.None;
		}
	}

	// Use this for initialization
	void Start () {
		IsLongPressed = new bool[4] {true, true, true, true};
		CurInput = new InputType[4];
		GameObject.Find("NetworkManager").GetComponent<NetworkConnector>().OnRead = OnSocketRead;
	}

	// Update is called once per frame
	void Update () {
		int curTime = (int)System.Math.Round(Time.timeSinceLevelLoad * 1000000) - BeatGen.StartTime;
		UpdateInput();
		// find first unpressed note in VALID State for player
		Note target = BeatGen.NoteList.First(n => (!n.IsPressed(Player)));
		if(target == null) return;
		// if button is up or down, send event
		if(CurInput[0] == InputType.DOWN || CurInput[0] == InputType.UP)
			target.Press(Player, curTime, Note.Button.RED, CurInput[0]);
		else if(CurInput[1] == InputType.DOWN || CurInput[1] == InputType.UP)
			target.Press(Player, curTime, Note.Button.BLUE, CurInput[1]);
		else if(CurInput[2] == InputType.DOWN || CurInput[2] == InputType.UP)
			target.Press(Player, curTime, Note.Button.GREEN, CurInput[2]);
		else if(CurInput[3] == InputType.DOWN || CurInput[3] == InputType.UP)
			target.Press(Player, curTime, Note.Button.SKILL, CurInput[3]);
		// if button is keep at appropriate time, send event
		else if(System.Math.Abs(target.Time - curTime) <= KEEP_MARGIN) {
			if(CurInput[0] == InputType.KEEP)
				target.Press(Player, curTime, Note.Button.RED, CurInput[0]);
			else if(CurInput[1] == InputType.KEEP)
				target.Press(Player, curTime, Note.Button.BLUE, CurInput[1]);
			else if(CurInput[2] == InputType.KEEP)
				target.Press(Player, curTime, Note.Button.GREEN, CurInput[2]);
			else if(CurInput[3] == InputType.KEEP)
				target.Press(Player, curTime, Note.Button.SKILL, CurInput[3]);
		}
		// if button is accepted, reset long button detector
		if(target.IsPressed(Player)) ResetLongButton();
	}

	private void ResetLongButton() {
		IsLongPressed[0] = IsLongPressed[1] = IsLongPressed[2] = IsLongPressed[3] = true;
	}

	private void UpdateInput() {
		// update IsLongPressed
		if(CurInput[0] == InputType.UP) IsLongPressed[0] = false;
		if(CurInput[1] == InputType.UP) IsLongPressed[1] = false;
		if(CurInput[2] == InputType.UP) IsLongPressed[2] = false;
		if(CurInput[3] == InputType.UP) IsLongPressed[3] = false;
		// process red button
		if(Input.GetKeyDown(ButToKeycode(Note.Button.RED)))
			CurInput[0] = InputType.DOWN;
		else if(Input.GetKeyUp(ButToKeycode(Note.Button.RED))) {
			CurInput[0] = (IsLongPressed[0])
						  ? InputType.UP
						  : InputType.NONE;
		}
		else if(Input.GetKey(ButToKeycode(Note.Button.RED)))
			CurInput[0] = (IsLongPressed[0])
						  ? InputType.KEEP
						  : InputType.NONE;
		else
			CurInput[0] = InputType.NONE;
		// process blue button
		if(Input.GetKeyDown(ButToKeycode(Note.Button.BLUE)))
			CurInput[1] = InputType.DOWN;
		else if(Input.GetKeyUp(ButToKeycode(Note.Button.BLUE))){
			CurInput[1] = (IsLongPressed[1])
						  ? InputType.UP
						  : InputType.NONE;
		}
		else if(Input.GetKey(ButToKeycode(Note.Button.BLUE)))
			CurInput[1] = (IsLongPressed[1])
						  ? InputType.KEEP
						  : InputType.NONE;
		else
			CurInput[1] = InputType.NONE;
		// process green button
		if(Input.GetKeyDown(ButToKeycode(Note.Button.GREEN)))
			CurInput[2] = InputType.DOWN;
		else if(Input.GetKeyUp(ButToKeycode(Note.Button.GREEN))){
			CurInput[2] = (IsLongPressed[2])
						  ? InputType.UP
						  : InputType.NONE;
		}
		else if(Input.GetKey(ButToKeycode(Note.Button.GREEN)))
			CurInput[2] = (IsLongPressed[2])
						  ? InputType.KEEP
						  : InputType.NONE;
		else
			CurInput[2] = InputType.NONE;
		// process green button
		if(Input.GetKeyDown(ButToKeycode(Note.Button.SKILL)))
			CurInput[3] = InputType.DOWN;
		else if(Input.GetKeyUp(ButToKeycode(Note.Button.SKILL))){
			CurInput[3] = (IsLongPressed[3])
						  ? InputType.UP
						  : InputType.NONE;
		}
		else if(Input.GetKey(ButToKeycode(Note.Button.SKILL)))
			CurInput[3] = (IsLongPressed[3])
						  ? InputType.KEEP
						  : InputType.NONE;
		else
			CurInput[3] = InputType.NONE;
	}
}