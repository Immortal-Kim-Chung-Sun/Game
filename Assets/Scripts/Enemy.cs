using UnityEngine;

public abstract class Enemy : Actor
{
	/// <summary>
	/// 적 사망 시 플레이어가 획득하는 점수
	/// </summary>
	[SerializeField] private int score;

	[Header("Cashing")]
	// 적 사운드 저장 클래스
	[SerializeField] protected EnemySound enemySound;

	/// <summary>
	/// 적의 공격 여부
	/// </summary>
	protected bool canAttack = false;

	protected Transform playerTransform;

	protected override void Awake()
	{
		base.Awake();

		playerTransform = FindObjectOfType<Player>().transform;
	}

	private void OnDestroy()
	{
		// 씬이 이동되면 플레이어가 적을 삭제하지 않아도 알아서 적이 삭제된다.
		// 따라서 플레이어가 있을 때, 즉 게임이 진행 중일 때만 점수를 획득하게 조건문을 달았다.
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
