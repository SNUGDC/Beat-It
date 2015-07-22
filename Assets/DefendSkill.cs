public class DefendSkill {
	public enum DefendState {GUARD, CANCEL, FAIL}
	public delegate DefendState DefendHandler(string attackSkill,
											  int turn,
											  bool defendableJudge);

	public string Name;
	public float DefendRate;
	public uint Damage;
	public DefendHandler DoDefend;
}