using UnityEngine;

public class SpecialSkill {
	public Player.PlayAnim PlayAnim;
	public string Name;
	public SpecialSkill(string name) {
		Name = name;
		PlayAnim = (Animator target, uint combo, bool isUp) => {
			target.Play(name);
		};
	}
}