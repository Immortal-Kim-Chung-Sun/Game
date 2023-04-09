using UnityEngine;

public abstract class Enemy : Actor
{
	[SerializeField] private int score;

	[Header("Cashing")]
	[SerializeField] protected EnemySound enemySound;

	protected bool canAttack = false;

	protected Transform playerTransform;

	protected override void Awake()
	{
		base.Awake();

		playerTransform = FindObjectOfType<Player>().transform;
	}

	private void OnDestroy()
	{
		if (playerTransform != null)
		{
			Score.score += score;
		}
	}

	protected override void DeathSound()
	{
		audioSource.clip = enemySound.death;
		audioSource.time = 0.5f;
		audioSource.Play();
	}
}
