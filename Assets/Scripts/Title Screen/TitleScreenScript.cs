using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreenScript : MonoBehaviour {

	public Sprite spriteTutorial;
	public Sprite spriteMainMenu;

	private Image imageRenderer;
	
	void Start()
	{
		imageRenderer = GetComponent<Image> ();
		if (imageRenderer.sprite == null)
			imageRenderer.sprite = spriteTutorial;
	}

	void Update()
	{
		// both left and right button (because we only have two options)
		if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown (KeyCode.RightArrow)) {
			ChangeMainMenu();
		}

		if (Input.GetKeyDown (KeyCode.Return)) {
			if (imageRenderer.sprite == spriteTutorial)
				GoToTutorial();
			else if(imageRenderer.sprite = spriteMainMenu)
				StartCoroutine("GoToSelectScreen");
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			ExitGame();
		}
	}

	void ChangeMainMenu()
	{
		imageRenderer.sprite = (imageRenderer.sprite == spriteTutorial) ? spriteMainMenu : spriteTutorial;
	}
	IEnumerator GoToSelectScreen()
	{
		GetComponent<Animator> ().SetBool ("fadeOut", true);
		yield return new WaitForSeconds (1.0f);
		Application.LoadLevel ("sceneSelect");
	}
	void GoToTutorial()
	{
		// Application.LoadLevel("sceneTutorial");
	}
	void ExitGame()
	{
		Application.Quit ();
	}
}
