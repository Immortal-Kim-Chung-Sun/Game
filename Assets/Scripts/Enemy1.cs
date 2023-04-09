using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Enemy
{
	protected override void Attack()
	{
		if (!canAttack) return;

		if (countAttackDelay > 0) return;

		countAttackDelay = attackDelay;

		animator.SetTrigger("Attack");

		audioSource.PlayOneShot(enemySound.attack[0]);

		RaycastHit2D boxCast = Physics2D.BoxCast(transform.position, new Vector2(1f, 2f), 0, spriteRenderer.flipX ? Vector2.right : Vector2.left, attackRange, LayerMask.GetMask("Player"));
		if (boxCast.transform != null)
		{
			boxCast.collider.GetComponent<Player>().BeShot(damage);
		}
	}

	protected override void Move()
	{
		float range = playerTransform.position.x - transform.position.x;
		canAttack = Mathf.Abs(range) <= attackRange;
		spriteRenderer.flipX = range > 0;
		transform.position += new Vector3(spriteRenderer.flipX ? Time.deltaTime : -Time.deltaTime, 0) * speed;
	}
}
