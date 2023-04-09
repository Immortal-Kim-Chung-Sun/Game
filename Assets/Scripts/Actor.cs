using System.Collections;
using UnityEngine;

// 적과 플레이어(김충선)의 공통점을 담은 추상화 클래스
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

	// 자식의 기능 추가를 위해 가상화
	protected virtual void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		rigidbody2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
	}

	// 자식의 기능 추가를 위해 가상화
	protected virtual void Update()
	{
		// 피격 시 공격이 가능하기까지 남은 시간이 줄어들지 않음
		if (hit) return;

		if (countAttackDelay > 0)
		{
			countAttackDelay -= Time.deltaTime;
		}

		Move();

		Attack();
	}

	/// <summary>
	/// 넉백 애니메이션 및 피격한 대상의 반대 방향으로 밀려남
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
	/// 오브젝트의 사망과 함께 소리 출력
	/// </summary>
	protected abstract void DeathSound();

	/// <summary>
	/// 오브젝트의 이동
	/// </summary>
	protected abstract void Move();

	/// <summary>
	/// 다른 오브젝트에서 이 함수를 호출하면 넉백 및 체력 감소
	/// </summary>
	/// <param name="damage"></param>
	public void BeShot(int damage)
	{
		// 전역변수에 현재 실행하고 있는 코루틴을 담아 코루틴 중복 실행 방지
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
	/// 오브젝트의 공격
	/// </summary>
	protected abstract void Attack();
}