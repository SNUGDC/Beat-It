using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	public enum InputResult {NONE, DOWN, KEEP, UP}

	public InputResult[] Result;
	private bool[] isLongPressed;

	// Use this for initialization
	void Start () {
		PreviousPressed = new bool[3] {false, false, false};
		Result = new InputResult[3] {InputManager.InputResult.NONE,
									 InputManager.InputResult.NONE,
									 InputManager.InputResult.NONE};
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Ready() {
		
	}
}