public class DefendSkill {
	public enum DefendState {NONE, GUARD, CANCEL, HIT, SKILL}
	public delegate DefendState DefendHandler(string attackSkill,
											  bool defendableJudge,
											  bool isUp);

	public string Name;
	public float DefendRate;
	public uint Damage;
	public uint SkillPoint;
	public DefendHandler DoDefend;
	public Player.PlayAnim PlayAnim;
}