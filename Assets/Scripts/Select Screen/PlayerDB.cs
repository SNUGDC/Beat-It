using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Skill {
	public Sprite sprite;
	public string name;
	public string koreanName;
	public string description;
	public uint length;
	public uint sp;
}

[System.Serializable]
public class Character {
	public Sprite sprite;
	public string name;
	public List<string> skillNames;
	[HideInInspector] public List<Skill> skills;

	void Start()
	{
		skills = new List<Skill> ();
	}
}

public class PlayerDB : MonoBehaviour {
	public List<Skill> skills;
	public List<Character> characters;

	public void Start()
	{
		foreach (Character character in characters) {
			foreach (string skillName in character.skillNames) {
				character.skills.Add (FindSkillFromName(skillName));
			}
		}
		DontDestroyOnLoad (transform.gameObject);
	}

	Skill FindSkillFromName(string name)
	{
		foreach (Skill skill in skills) {
			if (skill.name == name) return skill;
		}
		Debug.Log ("Skill names don't match! Check code.");
		return null;
	}
}
