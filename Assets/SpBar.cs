using UnityEngine;
using System.Collections;

public class SpBar : MonoBehaviour {
	public enum Direction {LEFT, RIGHT};
	public const float SPEED = 14;
	public const int MAX_SP = 10000;

	public RectTransform Content;
	public Direction MoveDir;
	public int CurVal {
		get { return _CurVal; }
	}
	private int _CurVal;
	private int Target;

	// Use this for initialization
	void Start () {
		Target = _CurVal = 0;
	}

	// Update is called once per frame
	void Update () {
		float width = GetComponent<RectTransform>().rect.width;
		float threshold = SPEED / width * MAX_SP;
		float distance = 0;
		if(_CurVal > Target + threshold) { 
			distance = SPEED;
			_CurVal -= (int)threshold;
		}
		else if(_CurVal < Target - threshold) {
			distance = -SPEED;
			_CurVal += (int)threshold;
		}
		else {
			distance = (float)(_CurVal - Target) / MAX_SP * width;
			_CurVal = Target;
		}
		distance = (MoveDir == Direction.LEFT) ? distance : -distance;
		this.transform.Translate(distance, 0, 0);
		Content.Translate(-distance, 0, 0);
	}

	public void Increase(int data) {
		Target += data;
		if(Target > MAX_SP) Target = MAX_SP;
		else if(Target < 0) Target = 0;
	}
}
