public class DefendSkill {
	public enum DefendState {NONE, GUARD, CANCEL, HIT}
	public delegate DefendState DefendHandler(string attackSkill,
											  bool defendableJudge,
											  bool isUp);

	public string Name;
	public float DefendRate;
	public uint Damage;
	public DefendHandler DoDefend;
	public Player.PlayAnim PlayAnim;
}