using UnityEngine;

public class Enemy : Actor
{
	public bool BeAttacked
	{
		get { return beAttacked; }
		set { beAttacked = value; }
	}

	[SerializeField]
	protected AudioClip[] _audioClip;

	protected bool canAttack = false;

	protected Transform playerTransform;

	protected override void Awake()
	{
		base.Awake();

		playerTransform = FindObjectOfType<Player>().transform;

		
	}

	private void OnDestroy()
	{
		if (FindObjectOfType<Player>() != null)
		{
			Score.score += (enemyNumber + 1) * 1000;
		}
	}
}
