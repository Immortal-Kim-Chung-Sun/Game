using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
	[SerializeField] private GameObject thorn;

	[Header("Boss Value")]
	[SerializeField] private float thornAttack = 5;

	private float countThornAttack;

	protected override void Update()
	{
		base.Update();

		if (countThornAttack > 0)
		{
			countThornAttack -= Time.deltaTime;
		}

		ThornAttack();
	}

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

	private void ThornAttack()
	{
		if (countThornAttack > 0) return;

		countThornAttack = thornAttack;

		StartCoroutine(GetDamage());
	}

	protected override void Move()
	{
		float range = playerTransform.position.x - transform.position.x;
		canAttack = Mathf.Abs(range) <= attackRange;
		spriteRenderer.flipX = range > 0;
		transform.position += new Vector3(spriteRenderer.flipX ? Time.deltaTime : -Time.deltaTime, 0) * speed;
	}

	private IEnumerator GetDamage()
	{
		audioSource.PlayOneShot(enemySound.attack[1]);

		yield return new WaitForSeconds(1f);

		GameObject thornClone = Instantiate(thorn, new Vector3(65, -3.5f, 0), Quaternion.identity);

		yield return new WaitForSeconds(0.5f);

		Destroy(thornClone);
	}
}
