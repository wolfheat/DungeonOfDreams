using UnityEngine;

public class Stats : MonoBehaviour
{
	public float miningSpeed;
	public const float MiningSpeedDefault = 3f;
	public const float MiningSpeedSpeedUp = 12f;

	public static Stats Instance { get; private set; }

	private void Start()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		miningSpeed = MiningSpeedDefault;
    }

	public void SetDefaultMiningSpeed()
	{
		miningSpeed = MiningSpeedDefault;
	}
	public void SetBoostMiningSpeed()
	{
		miningSpeed = MiningSpeedSpeedUp;
	}

}
