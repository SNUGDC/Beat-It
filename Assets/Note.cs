using UnityEngine;

public class Note{
	public struct Core {
		public uint Id;
		public Button Button;
		public InputManager.InputType Type;
		public uint Judge;
	}
	public enum Button {NONE, RED, BLUE, GREEN, SKILL};

	public bool IsValid; // is shown in display
	public int Time; // time of note

	private Core[] CoreData;
	private bool Flip;

	public bool IsPressed(int player) {
		return CoreData[player].Button != Button.NONE;
	}

	// default constructor
	public Note(uint id, int time, bool flip) {
		CoreData = new Core[2];
		IsValid = false;
		Time = time;
		CoreData[0].Id = CoreData[1].Id = id;
		CoreData[0].Button = CoreData[1].Button = Button.NONE;
		CoreData[0].Type = CoreData[1].Type = InputManager.InputType.NONE;
		CoreData[0].Judge = CoreData[1].Judge = 0;
		Flip = flip;
	}

	// create note on display
	public void Appear(Transform notePrefab) {
		Transform newTrans = Object.Instantiate(
			notePrefab,
			new Vector3(0, -19.72f, -1 + 0.001f * CoreData[0].Id),
			Quaternion.identity
		) as Transform;
		IsValid = true;
		newTrans.parent = GameObject.Find("BeatGenerator").transform;
		newTrans.GetComponent<NoteMover>().NoteData = this;
	}

	// accept user input
	public void Press(int player, int time, Button but, InputManager.InputType type) {
		var manager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
		float judge = 1 - System.Math.Abs(time - this.Time) / (float)(BeatGenerator.BEAT_MARGIN);
		// if timing is bad & UP signal is received
		if(judge <= 0 && type == InputManager.InputType.UP) {
			manager.PressUp(player, but);
		}
		// if timing is good & is valid input, accept
		else if(judge > 0 && manager.IsValidInput(player, but, type)){
			manager.GetReady(player, but, type);
			CoreData[player].Button = but;
			CoreData[player].Type = type;
			CoreData[player].Judge = (uint)(judge * 1000);
		}
	}

	// kill note from display
	// calls actual battle logic
	public void Kill() {
		var network = GameObject.Find("NetworkManager").GetComponent<NetworkConnector>();
		var manager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
		if(network.LocalPlayer[0]) manager.DataQueue[0].Enqueue(CoreData[0]);
		else {
			network.SendString("0"
							   + ' ' + CoreData[0].Id.ToString()
							   + ' ' + CoreData[0].Button.ToString()
							   + ' ' + CoreData[0].Type.ToString()
							   + ' ' + CoreData[0].Judge.ToString());
		}
		if(network.LocalPlayer[1]) manager.DataQueue[1].Enqueue(CoreData[1]);
		else {
			network.SendString("1"
							   + ' ' + CoreData[1].Id.ToString()
							   + ' ' + CoreData[1].Button.ToString()
							   + ' ' + CoreData[1].Type.ToString()
							   + ' ' + CoreData[1].Judge.ToString());
		}
		// call actual battle logic
		manager.StartCoroutine(manager.DoBattle(CoreData[0].Id, Flip));
	}
}