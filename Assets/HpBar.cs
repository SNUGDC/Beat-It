using UnityEngine;
using System.Collections;

public class HpBar : MonoBehaviour {
	public enum Direction {LEFT, RIGHT};
	public const float SPEED = 14;
	public const int MAX_HP = 300;

	public Transform Content;
	public Direction MoveDir;
	public int CurVal {
		get { return _CurVal; }
	}
	private int _CurVal;
	private int Target;

	// Use this for initialization
	void Start () {
		Target = _CurVal = MAX_HP;
	}

	// Update is called once per frame
	void Update () {
		float width = GetComponent<RectTransform>().rect.width;
		float threshold = SPEED / width * MAX_HP;
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
			distance = (float)(_CurVal - Target) / MAX_HP * width;
			_CurVal = Target;
		}
		distance = (MoveDir == Direction.RIGHT) ? distance : -distance;
		this.transform.Translate(distance * Time.deltaTime, 0, 0);
		Content.Translate(-distance * Time.deltaTime, 0, 0);
	}

	public void Increase(int data) {
		Target += data;
		if(Target > MAX_HP) Target = MAX_HP;
		else if(Target < 0) Target = 0;
	}
}