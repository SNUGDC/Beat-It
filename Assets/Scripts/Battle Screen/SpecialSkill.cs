using UnityEngine;

public class SpecialSkill {
	public Skill skill;
	public Player.PlayAnim PlayAnim;
	public SpecialSkill(Skill skill)
	{
		PlayAnim = (Animator target, uint combo, bool isUp) => {
			target.Play(skill.name);
		};
	}
}