using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public abstract class Actor : MonoBehaviour
{
	[Header("Value")]
	[SerializeField] protected int hp;
	[SerializeField] protected int damage;
	[SerializeField] protected float speed;
	[SerializeField] protected float attackRange;
	[SerializeField] protected float attackDelay;
	[SerializeField] protected float knockBackRange;

	protected bool hit = false;

	protected float countAttackDelay = 0f;

	protected Animator animator;
	protected SpriteRenderer spriteRenderer;
	protected Rigidbody2D rigidbody2d;
	protected AudioSource audioSource;

	private Coroutine knockBackCoroutine = null;

	protected virtual void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		rigidbody2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
	}

	protected virtual void Update()
	{
		if (hit) return;

		if (countAttackDelay > 0)
		{
			countAttackDelay -= Time.deltaTime;
		}

		Move();

		Attack();
	}

	protected virtual IEnumerator KnockBack()
	{
		hit = true;

		animator.SetBool("KnockBack", true);

		rigidbody2d.AddForce(new Vector2(spriteRenderer.flipX ? -knockBackRange : knockBackRange, 0), ForceMode2D.Impulse);

		yield return new WaitForSeconds(0.2f);

		animator.SetBool("KnockBack", false);

		hit = false;

		knockBackCoroutine = null;
	}

	protected abstract void DeathSound();

	protected abstract void Move();

	public void BeShot(int damage)
	{
		if (knockBackCoroutine != null)
		{
			StopCoroutine(knockBackCoroutine);
		}
		knockBackCoroutine = StartCoroutine(KnockBack());

		hp -= damage;

		if (hp <= 0)
		{
			DeathSound();
			Destroy(gameObject);
		}
	}

	protected abstract void Attack();
}