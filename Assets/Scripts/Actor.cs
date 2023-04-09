using System.Collections;
using UnityEngine;

// ���� �÷��̾�(���漱)�� �������� ���� �߻�ȭ Ŭ����
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

	// �ڽ��� ��� �߰��� ���� ����ȭ
	protected virtual void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		rigidbody2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
	}

	// �ڽ��� ��� �߰��� ���� ����ȭ
	protected virtual void Update()
	{
		// �ǰ� �� ������ �����ϱ���� ���� �ð��� �پ���� ����
		if (hit) return;

		if (countAttackDelay > 0)
		{
			countAttackDelay -= Time.deltaTime;
		}

		Move();

		Attack();
	}

	/// <summary>
	/// �˹� �ִϸ��̼� �� �ǰ��� ����� �ݴ� �������� �з���
	/// </summary>
	/// <returns></returns>
	private IEnumerator KnockBack()
	{
		hit = true;

		animator.SetBool("KnockBack", true);

		rigidbody2d.AddForce(new Vector2(spriteRenderer.flipX ? -knockBackRange : knockBackRange, 0), ForceMode2D.Impulse);

		yield return new WaitForSeconds(0.2f);

		animator.SetBool("KnockBack", false);

		hit = false;

		knockBackCoroutine = null;
	}

	/// <summary>
	/// ������Ʈ�� ����� �Բ� �Ҹ� ���
	/// </summary>
	protected abstract void DeathSound();

	/// <summary>
	/// ������Ʈ�� �̵�
	/// </summary>
	protected abstract void Move();

	/// <summary>
	/// �ٸ� ������Ʈ���� �� �Լ��� ȣ���ϸ� �˹� �� ü�� ����
	/// </summary>
	/// <param name="damage"></param>
	public void BeShot(int damage)
	{
		// ���������� ���� �����ϰ� �ִ� �ڷ�ƾ�� ��� �ڷ�ƾ �ߺ� ���� ����
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
	
	/// <summary>
	/// ������Ʈ�� ����
	/// </summary>
	protected abstract void Attack();
}