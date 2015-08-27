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
	public Text nameText;
	private Image imageRenderer;
	
	public GameObject skillImagePrefab;

	// Use this for initialization
	void Start () {
		imageRenderer = GetComponent<Image> ();
		database = GameObject.Find ("PlayerDatabase").GetComponent<PlayerDB>();
		playerIDMax = database.characters.Count - 1;
		UpdateCharacter (playerID);
		ClearSkillImageObjects ();
		UpdateSkillImageObjects ();
		// correction of text scale
		var scale = readyText.gameObject.transform.localScale;
		scale.x = (playerType == 1)? -Mathf.Abs (scale.x) : Mathf.Abs (scale.x);
		readyText.gameObject.transform.localScale = scale;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		readyText.text = (isReady)? "Ready" : "Not Ready";
		nameText.text = "Character" + (playerID + 1).ToString();
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
			GameObject skillPanel = GameObject.Find ("SkillPanel " + playerType.ToString ());
			go.transform.SetParent(skillPanel.transform);
			// move skill image to desired position
			var panelWidth = skillPanel.GetComponent<RectTransform>().rect.width;
			var panelHeight = skillPanel.GetComponent<RectTransform>().rect.height;
			go.transform.localPosition = new Vector3(-0.37f * panelWidth, 0.0f, 0.0f);
			go.transform.localScale = Vector3.one;
			//go.GetComponent<RectTransform>().sizeDelta = new Vector2 (0.8f*panelHeight, 0.8f*panelHeight);
			// move skill image position horizontally depending on count
			//var iconWidth = go.GetComponent<RectTransform>().rect.width;
			Vector3 pos = go.transform.position;
			pos.x += panelHeight * 1.2f * count * Screen.height/600;
			go.transform.position = pos;
			count++;
		}
	}



	void ClearSkillImageObjects()
	{
		foreach (Transform childObject in GameObject.Find ("SkillPanel " + playerType.ToString ()).transform)
			Destroy (childObject.gameObject);
	}
}
