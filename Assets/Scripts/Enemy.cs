using UnityEngine;

public abstract class Enemy : Actor
{
	/// <summary>
	/// �� ��� �� �÷��̾ ȹ���ϴ� ����
	/// </summary>
	[SerializeField] private int score;

	[Header("Cashing")]
	// �� ���� ���� Ŭ����
	[SerializeField] protected EnemySound enemySound;

	/// <summary>
	/// ���� ���� ����
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
		// ���� �̵��Ǹ� �÷��̾ ���� �������� �ʾƵ� �˾Ƽ� ���� �����ȴ�.
		// ���� �÷��̾ ���� ��, �� ������ ���� ���� ���� ������ ȹ���ϰ� ���ǹ��� �޾Ҵ�.
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
