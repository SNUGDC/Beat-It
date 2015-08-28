using UnityEngine;
using System.Collections;

public class HpBar : MonoBehaviour {
	public enum Direction {LEFT, RIGHT};
	public const float SPEED = 14;
	public const int MAX_HP = 300;

	public RectTransform Content;
	public Direction MoveDir;
	public int Value {
		get { return (int)Target; }
	}
	private float CurVal;
	private float Target;

	// Use this for initialization
	void Start () {
		Target = CurVal = MAX_HP;
	}

	// Update is called once per frame
	void Update () {
		float width = GetComponent<RectTransform>().rect.width;
		float threshold = SPEED / width * MAX_HP;
		float distance = 0;
		if(CurVal > Target + threshold) { 
			distance = SPEED;
			CurVal -= threshold;
		}
		else if(CurVal < Target - threshold) {
			distance = -SPEED;
			CurVal += threshold;
		}
		else {
			distance = (CurVal - Target) / MAX_HP * width;
			CurVal = Target;
		}
		distance = (MoveDir == Direction.RIGHT) ? distance : -distance;
		GetComponent<RectTransform>().anchoredPosition += new Vector2(distance, 0);
		Content.anchoredPosition += new Vector2(-distance, 0);
	}

	public void Increase(int data) {
		Target += data;
		if(Target > MAX_HP) Target = MAX_HP;
		else if(Target < 0) Target = 0;
	}
}