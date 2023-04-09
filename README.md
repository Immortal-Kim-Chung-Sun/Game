# 불멸의 김충선(Immortal Kim Chung Sun)

<h4>2학년 1학기 역사 수행평가 (조선 장군 김충선이 왜적을 무찌르는 2D 플래시 액션 게임)</h4>

<hr class='hr-solid'/>

<h4>게임 시연</h4>

<A href=""> 게임 시연 영상 </A><br><p>

<hr class='hr-solid'/>

<h3>시스템 구조</h3>

<details>
<summary><i>인 게임 이미지</i></summary>
<br>
 - 타이틀<br>
  <img width="640" alt="image" src="https://user-images.githubusercontent.com/80941288/230759368-07246235-f115-4f7d-93c6-b6b5bccf9397.png"><br>
  <br>
 - 플레이<br>
  <img width="640" alt="image" src="https://user-images.githubusercontent.com/80941288/230759394-b92adaa6-1b34-4063-a3f5-3d218546f081.png"><br>
  <img width="640" alt="image" src="https://user-images.githubusercontent.com/80941288/230759420-7735cc43-685b-4ea0-af3f-a1eea5f05b9a.png"><br>
  <img width="640" alt="image" src="https://user-images.githubusercontent.com/80941288/230759522-a6f811c8-4585-45f7-bef0-cb19c3d7441c.png"><br>
  <br>
 - 끝<br>
  <img width="640" alt="image" src="https://user-images.githubusercontent.com/80941288/230759537-d521dcab-3550-4abf-909a-8a65e0b7304c.png"><br>
  <br>
</details>

<img width="640" alt="image" src="https://user-images.githubusercontent.com/80941288/230767508-894b1542-da17-4542-94c1-bb2c5101f853.png"><br>

<hr class='hr-solid'/>

<h3>주요 코드</h3>
<b>Actor</b><br>
&nbsp;&nbsp;&nbsp;&nbsp;● 플레이어(김충선)과 적의 모체가 되는 클래스입니다.<br>
&nbsp;&nbsp;&nbsp;&nbsp;● abstract 클래스로 상속 후 무조건 공격, 이동, 사망 시 사운드 함수를 구현해야합니다.<br>
<details>
    <summary><i>자세한 코드</i></summary>
    
  ```C#
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

	// 넉백 애니메이션 및 피격한 대상의 반대 방향으로 밀려남
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

	// 오브젝트의 사망과 함께 소리 출력
	protected abstract void DeathSound();

	// 오브젝트의 이동
	protected abstract void Move();

	// 다른 오브젝트에서 이 함수를 호출하면 넉백 및 체력 감소
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
	
	// 오브젝트의 공격
	protected abstract void Attack();
}
  ```
</details><br>

<b>Player</b><br>
&nbsp;&nbsp;&nbsp;&nbsp;● 플레이어의 이동 및 공격을 담당하는 클래스입니다.<br>
<details>
    <summary><i>자세한 코드</i></summary>
    
  ```C#
  using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// 추상화 클래스 Actor 상속
public class Player : Actor
{
	[SerializeField] private float jumpRange;
	public bool canJump = false;

	[Header("Cashing")]
	[SerializeField] private AudioClip attackSound;
	[SerializeField] private Image hpBar;
	[SerializeField] private TextMeshProUGUI hpText;
	[SerializeField] private Combo combo;

	private int maxHP;

	protected override void Awake()
	{
		base.Awake();

		maxHP = hp;

		animator = GetComponent<Animator>();

		audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
	}

	protected override void Update()
	{
		animator.SetBool("Walk", false);

		base.Update();

		Jump();

		// UI에 HP가 얼마나 남았는지 이미지와 텍스트로 표현
		hpBar.fillAmount = hp / (float)maxHP;

		hpText.text = hp + "/" + maxHP;
	}

	// 플레이어의 이동
	protected override void Move()
	{
		// 왼쪽 화살표 입력 시, 이미지가 왼쪽으로 바라보게 하고 왼쪽 이동
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			spriteRenderer.flipX = true;
			transform.Translate(speed * Time.deltaTime * Vector2.left);

			animator.SetBool("Walk", true);
		}
		// 오른쪽 화살표 입력 시, 이미지가 오른쪽으로 바라보게 하고 오른쪽 이동
		if (Input.GetKey(KeyCode.RightArrow))
		{
			spriteRenderer.flipX = false;
			transform.Translate(speed * Time.deltaTime * Vector2.right);

			animator.SetBool("Walk", true);
		}
	}

	// 플레이어의 점프
	private void Jump()
	{
		// 이미 점프를 하면 점프 사용 불가능
		if (!canJump) return;

		// 위 화살표 입력 시, 실행
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			rigidbody2d.AddForce(new Vector2(0, jumpRange), ForceMode2D.Impulse);
			canJump = false;
		}
	}

	protected override void Attack()
	{
		if (countAttackDelay > 0) return;

		// A 키가 눌렸을 때
		if (Input.GetKeyDown(KeyCode.A))
		{
			// 공격 딜레이 초기화
			countAttackDelay = attackDelay;
			
			// 애니메이션 출력
			animator.SetTrigger("Attack");

			// 오디오 출력
			audioSource.clip = attackSound;
			audioSource.time = 0.3f;
			audioSource.Play();

			// 공격 판정 생성, 만약 범위 내에 오브젝트가 있다면 boxCast 변수에 저장
			RaycastHit2D boxCast = Physics2D.BoxCast(transform.position, new Vector2(1, 3f), 0, spriteRenderer.flipX ? Vector2.left : Vector2.right, attackRange, LayerMask.GetMask("Enemy"));
			// boxCast에 저장된 값이 있다면, 적이 피격된 것으로 판단. 적에게 넉백 및 데미지를 가함
			if (boxCast.transform != null)
			{
				combo.Intactly();
				boxCast.collider.GetComponent<Enemy>().BeShot(damage);
			}
		}
	}

	// [사용하지 않음] 사망 시, 바로 끝 Scene으로 가기 때문
	protected override void DeathSound()
	{

	}

	// 오브젝트 삭제 시, End Scene으로 이동
	private void OnDestroy()
	{
		SceneManager.LoadScene("End");
	}

	// 오브젝트가 닿았을 시, 발동하는 함수
	private void OnTriggerEnter2D(Collider2D collision)
	{
		// 바닥에 닿았는지 확인하여 점프 여부를 체크
		if (collision.CompareTag("Ground"))
		{
			canJump = true;
		}

		// 가시에 닿으면 체력 감소 및 넉백
		if (collision.CompareTag("Thorn"))
		{
			BeShot(5);
		}

		// 총알에 피격 시, 피격 당한 총알 삭제 및 체력 감소, 넉백
		if (collision.CompareTag("Bullet"))
		{
			BeShot(collision.GetComponent<Bullet>().damage);
			Destroy(collision.gameObject);
		}
	}

}
  ```
</details><br>
