using UnityEngine;

// �߻�ȭ Ŭ���� Enemy ���
public class Enemy1 : Enemy
{
	/// <summary>
	/// RayCast�� ���� ���� �÷��̾ �ִ��� Ȯ�� �� ����
	/// </summary>
	protected override void Attack()
	{
		// ������ ����ٸ� �Լ� ����
		if (!canAttack) return;

		// �����̰� �����ִٸ� �Լ� ����
		if (countAttackDelay > 0) return;

		// ���� ����. ������ �ʱ�ȭ
		countAttackDelay = attackDelay;

		// �ִϸ��̼� ���
		animator.SetTrigger("Attack");

		// ���� ���
		audioSource.PlayOneShot(enemySound.attack[0]);

		// RayCast�� ���� ���� ����. ���� ���� ���� �÷��̾ �ִٸ� boxCast�� �� ����
		RaycastHit2D boxCast = Physics2D.BoxCast(transform.position, new Vector2(1f, 2f), 0, spriteRenderer.flipX ? Vector2.right : Vector2.left, attackRange, LayerMask.GetMask("Player"));
		// boxCast�� ���� ����Ǿ��ִٸ�, �÷��̾�� �˹� �� �������� ����
		if (boxCast.transform != null)
		{
			boxCast.collider.GetComponent<Player>().BeShot(damage);
		}
	}

	/// <summary>
	/// �÷��̾ ���󰡴� AI�� ������� �̵�
	/// </summary>
	protected override void Move()
	{
		float range = playerTransform.position.x - transform.position.x;
		// �÷��̾���� �Ÿ��� ������ �� �ִ� �Ÿ����� Ȯ�� �� Bool ������ ����
		canAttack = Mathf.Abs(range) <= attackRange;
		// �÷��̾��� ��ġ�� ���� �̹��� ���� ��ȯ
		spriteRenderer.flipX = range > 0;
		// �÷��̾ ���� �̵�
		transform.position += new Vector3(spriteRenderer.flipX ? Time.deltaTime : -Time.deltaTime, 0) * speed;
	}
}