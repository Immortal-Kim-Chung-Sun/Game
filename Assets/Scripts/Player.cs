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

	/// <summary>
	/// 플레이어의 이동
	/// </summary>
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

	/// <summary>
	/// 플레이어의 점프
	/// </summary>
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

	/// <summary>
	/// [사용하지 않음] 사망 시, 바로 끝 Scene으로 가기 때문
	/// </summary>
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