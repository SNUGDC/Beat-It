using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {

	PlayerDB database;
	Character character;
	public int playerID = 0;
	private int playerIDMax;
	public int playerType;
	[HideInInspector] public bool isReady = false;

	public Text readyText;
	private Image imageRenderer;
	
	public GameObject skillImagePrefab;

	// Use this for initialization
	void Start () {
		imageRenderer = GetComponent<Image> ();
		database = GameObject.Find ("PlayerDatabase").GetComponent<PlayerDB>();
		playerIDMax = database.characters.Count - 1;
		UpdateCharacter (playerID);
		UpdateSkillImageObjects ();
		// correction of text scale
		var scale = readyText.gameObject.transform.localScale;
		scale.x = (playerType == 1)? -Mathf.Abs (scale.x) : Mathf.Abs (scale.x);
		readyText.gameObject.transform.localScale = scale;
	}
	
	// Update is called once per frame
	void Update () {
		readyText.text = (isReady)? "Ready" : "Not Ready";

		bool isChanged = false;
		if ((playerType == 1)? Input.GetKeyDown (KeyCode.A) : Input.GetKeyDown (KeyCode.J)) {
			playerID = (playerID == 0)? playerIDMax : playerID - 1;
			isChanged = true;
		} else if ((playerType == 1)? Input.GetKeyDown (KeyCode.D) : Input.GetKeyDown (KeyCode.L)) {
			playerID = (playerID == playerIDMax)? 0 : playerID + 1;
			isChanged = true;
		} else if ((playerType == 1)? Input.GetKeyDown (KeyCode.Q) : Input.GetKeyDown(KeyCode.P)) {
			isReady = !isReady;
		}
		if (isChanged) {
			UpdateCharacter (playerID);
			ClearSkillImageObjects();
			UpdateSkillImageObjects();
		}
	}

	void UpdateCharacter(int playerID)
	{
		character = database.characters[playerID];
		imageRenderer.sprite = character.sprite;
		Vector3 scale = transform.localScale;
		scale.x = (playerType == 1) ? -Mathf.Abs (transform.localScale.x) : Mathf.Abs (transform.localScale.x);
		transform.localScale = scale;
	}

	void UpdateSkillImageObjects()
	{
		int count = 0;
		foreach(Skill skill in character.skills) {
			GameObject go = Instantiate (skillImagePrefab) as GameObject;
			SkillImageScript script = go.GetComponent<SkillImageScript>();
			script.skillName = skill.name; script.description = skill.description;
			script.sprite = skill.sprite;
			script.Setup();
			go.transform.SetParent(transform.FindChild ("SkillPanel " + playerType.ToString ()));
			// move skill image to desired position
			go.transform.localPosition = (playerType == 1)? new Vector3(39f, 0f, 0f) : new Vector3(-59f, 0f, 0f);
			// set scale to normal again (because the scale of the child object inherits the parent)
			Vector3 scale = go.transform.localScale;
			scale.x = (playerType == 1) ? -Mathf.Abs (transform.localScale.x) : Mathf.Abs (transform.localScale.x);
			go.transform.localScale = scale;
			// move skill image position horizontally depending on count
			Vector3 pos = go.transform.position;
			pos.x += 50 * count;
			go.transform.position = pos;
			count++;
		}
	}

	void ClearSkillImageObjects()
	{
		foreach (Transform childObject in transform.FindChild ("SkillPanel " + playerType.ToString ()))
			Destroy (childObject.gameObject);
	}
}
