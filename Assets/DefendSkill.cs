public class DefendSkill {
	public enum DefendState {GUARD, CANCEL, HIT}
	public delegate DefendState DefendHandler(string attackSkill,
											  uint turn,
											  bool defendableJudge);

	public string Name;
	public float DefendRate;
	public uint Damage;
	public DefendHandler DoDefend;
}