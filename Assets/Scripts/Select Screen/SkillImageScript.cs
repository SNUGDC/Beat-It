using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SkillImageScript : MonoBehaviour {
	[HideInInspector] public string skillName;
	[HideInInspector] public string koreanSkillName;
	[HideInInspector] public string description;
	[HideInInspector] public Sprite sprite;

	private Image imageRenderer;
	private Button button;
	private Text textRenderer;
	private GameObject textLabel;

	private bool isLoaded = false;
	private bool isMouseOver = false;

	void Start()
	{
	}
	public void Setup()
	{
		isLoaded = true;
		imageRenderer = GetComponentInChildren<Image> ();
		textRenderer = GetComponentInChildren<Text> ();
		textLabel = transform.FindChild ("SkillTextPanel").gameObject;
		imageRenderer.sprite = sprite;
		textRenderer.text = koreanSkillName + "\n" + description.Replace ("\\n","\n");
		Color color = textRenderer.color;
		color.a = 0.0f;
		textRenderer.color = color;
		textLabel.SetActive (false);
	}

	void Update()
	{
		if (isLoaded) {
			Color color = textRenderer.color;
			color.a = (isMouseOver == true)? 1.0f : 0.0f;
			textRenderer.color = color;
		}
	}

	public void OnMouseEnter()
	{
		if (isLoaded) {
			isMouseOver = true;
			textLabel.SetActive(true);
		}
	}
	public void OnMouseExit()
	{
		if (isLoaded) {
			isMouseOver = false;
			textLabel.SetActive (false);
		}
	}
}
