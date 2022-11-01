using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Actor : MonoBehaviour
{
	public int hp;

	protected Animator animator;

	[SerializeField]
	protected float attackRange;

	[SerializeField]
	protected int enemyNumber;

	protected bool beAttacked = false;

	protected bool isHit = false;

	private float countDelay = 0f;

	[SerializeField]
	protected float attackDelay, knockBackRange;

	protected SpriteRenderer spriteRenderer;

	protected Rigidbody2D _rigidbody2D;

	protected virtual void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		_rigidbody2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		switch (enemyNumber)
		{
			case 0:
				hp = 3;
				break;
			case 1:
				hp = 5;
				break;
			case 2:
				hp = 30;
				break;
			default:
				break;
		}
	}

	protected virtual void Update()
	{
		if (beAttacked)
		{
			StartCoroutine(KnockBack());
			beAttacked = false;
		}

		if (isHit) return;

		countDelay += Time.deltaTime;

		Move();

		if (countDelay >= attackDelay)
		{
			Attack();
		}
	}

	protected virtual IEnumerator KnockBack()
	{
		isHit = true;

		animator.SetBool("KnockBack", true);

		_rigidbody2D.AddForce(new Vector2(spriteRenderer.flipX ? -knockBackRange : knockBackRange, 0), ForceMode2D.Impulse);

		yield return new WaitForSeconds(0.2f);
		animator.SetBool("KnockBack", false);

		hp--;

		if (hp is 0)
		{
			DeathSound();
			Destroy(gameObject);
		}


		isHit = false;
	}

	protected virtual void DeathSound()
	{

	}

	protected virtual void Move()
	{

	}

	protected virtual void Attack()
	{
		countDelay = 0f;
	}
}