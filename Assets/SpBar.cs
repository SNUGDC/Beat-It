using UnityEngine;
using System.Collections;

public class SpBar : MonoBehaviour {
	public enum Direction {LEFT, RIGHT};
	public const float SPEED = 14;
	public const int MAX_SP = 10000;

	public RectTransform Content;
	public Direction MoveDir;
	public int Value {
		get { return Target; }
	}
	private int CurVal;
	private int Target;

	// Use this for initialization
	void Start () {
		Target = CurVal = 0;
	}

	// Update is called once per frame
	void Update () {
		float width = GetComponent<RectTransform>().rect.width;
		float threshold = SPEED / width * MAX_SP;
		float distance = 0;
		if(CurVal > Target + threshold) { 
			distance = SPEED;
			CurVal -= (int)threshold;
		}
		else if(CurVal < Target - threshold) {
			distance = -SPEED;
			CurVal += (int)threshold;
		}
		else {
			distance = (float)(CurVal - Target) / MAX_SP * width;
			CurVal = Target;
		}
		distance = (MoveDir == Direction.LEFT) ? distance : -distance;
		GetComponent<RectTransform>().anchoredPosition += new Vector2(distance, 0);
		Content.anchoredPosition += new Vector2(-distance, 0);
	}

	public void Increase(int data) {
		Target += data;
		if(Target > MAX_SP) Target = MAX_SP;
		else if(Target < 0) Target = 0;
	}
}
