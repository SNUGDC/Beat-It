using UnityEngine;
using System.Collections;

public class InputEffectScript : MonoBehaviour {

	public SpriteRenderer spriteRenderer1;
	public SpriteRenderer spriteRenderer2;

	public Sprite[] spriteArray1;
	public Sprite[] spriteArray2;
	
	void Update () {
		if (Input.GetKey (KeyCode.A))
			spriteRenderer1.sprite = spriteArray1 [0];
		else if (Input.GetKey (KeyCode.S))
			spriteRenderer1.sprite = spriteArray1 [1];
		else if (Input.GetKey (KeyCode.D))
			spriteRenderer1.sprite = spriteArray1 [2];
		else
			spriteRenderer1.sprite = null;

		if (Input.GetKey (KeyCode.L))
			spriteRenderer2.sprite = spriteArray2 [0];
		else if (Input.GetKey (KeyCode.Semicolon))
			spriteRenderer2.sprite = spriteArray2 [1];
		else if (Input.GetKey (KeyCode.Quote))
			spriteRenderer2.sprite = spriteArray2 [2];
		else
			spriteRenderer2.sprite = null;
	}
}
