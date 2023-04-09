using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Enemy
{
	[SerializeField] private GameObject bullet;

	private bool shooting = false;

	protected override void Attack()
	{
		if (!canAttack) return;

		if (countAttackDelay > 0) return;

		countAttackDelay = attackDelay;

		StartCoroutine(Wait());
	}

	private IEnumerator Wait()
	{
		shooting = true;

		animator.SetBool("Attack", true);

		audioSource.PlayOneShot(enemySound.attack[0]);

		yield return new WaitForSeconds(0.5f);

		Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, spriteRenderer.flipX ? 0 : 180));
		
		animator.SetBool("Attack", false);

		shooting = false;
		canAttack = false;
	}

	protected override void Move()
	{
		if (shooting) return;

		float range = playerTransform.position.x - transform.position.x;
		canAttack = Mathf.Abs(range) <= attackRange;
		spriteRenderer.flipX = range > 0;
		transform.position += new Vector3(spriteRenderer.flipX ? Time.deltaTime : -Time.deltaTime, 0) * speed;
	}
}
