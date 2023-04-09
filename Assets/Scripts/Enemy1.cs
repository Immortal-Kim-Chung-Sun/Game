using UnityEngine;

// 추상화 클래스 Enemy 상속
public class Enemy1 : Enemy
{
	/// <summary>
	/// RayCast로 판정 내에 플레이어가 있는지 확인 후 공격
	/// </summary>
	protected override void Attack()
	{
		// 범위를 벗어났다면 함수 종료
		if (!canAttack) return;

		// 딜레이가 남아있다면 함수 종료
		if (countAttackDelay > 0) return;

		// 공격 시작. 딜레이 초기화
		countAttackDelay = attackDelay;

		// 애니메이션 출력
		animator.SetTrigger("Attack");

		// 사운드 출력
		audioSource.PlayOneShot(enemySound.attack[0]);

		// RayCast로 공격 판정 생성. 공격 판정 내에 플레이어가 있다면 boxCast에 값 저장
		RaycastHit2D boxCast = Physics2D.BoxCast(transform.position, new Vector2(1f, 2f), 0, spriteRenderer.flipX ? Vector2.right : Vector2.left, attackRange, LayerMask.GetMask("Player"));
		// boxCast에 값이 저장되어있다면, 플레이어에게 넉백 및 데미지를 가함
		if (boxCast.transform != null)
		{
			boxCast.collider.GetComponent<Player>().BeShot(damage);
		}
	}

	/// <summary>
	/// 플레이어를 따라가는 AI를 기반으로 이동
	/// </summary>
	protected override void Move()
	{
		float range = playerTransform.position.x - transform.position.x;
		// 플레이어와의 거리가 공격할 수 있는 거리인지 확인 후 Bool 변수에 저장
		canAttack = Mathf.Abs(range) <= attackRange;
		// 플레이어의 위치에 따라 이미지 방향 전환
		spriteRenderer.flipX = range > 0;
		// 플레이어를 향해 이동
		transform.position += new Vector3(spriteRenderer.flipX ? Time.deltaTime : -Time.deltaTime, 0) * speed;
	}
}