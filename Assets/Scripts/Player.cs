using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// �߻�ȭ Ŭ���� Actor ���
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

		// UI�� HP�� �󸶳� ���Ҵ��� �̹����� �ؽ�Ʈ�� ǥ��
		hpBar.fillAmount = hp / (float)maxHP;

		hpText.text = hp + "/" + maxHP;
	}

	/// <summary>
	/// �÷��̾��� �̵�
	/// </summary>
	protected override void Move()
	{
		// ���� ȭ��ǥ �Է� ��, �̹����� �������� �ٶ󺸰� �ϰ� ���� �̵�
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			spriteRenderer.flipX = true;
			transform.Translate(speed * Time.deltaTime * Vector2.left);

			animator.SetBool("Walk", true);
		}
		// ������ ȭ��ǥ �Է� ��, �̹����� ���������� �ٶ󺸰� �ϰ� ������ �̵�
		if (Input.GetKey(KeyCode.RightArrow))
		{
			spriteRenderer.flipX = false;
			transform.Translate(speed * Time.deltaTime * Vector2.right);

			animator.SetBool("Walk", true);
		}
	}

	/// <summary>
	/// �÷��̾��� ����
	/// </summary>
	private void Jump()
	{
		// �̹� ������ �ϸ� ���� ��� �Ұ���
		if (!canJump) return;

		// �� ȭ��ǥ �Է� ��, ����
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			rigidbody2d.AddForce(new Vector2(0, jumpRange), ForceMode2D.Impulse);
			canJump = false;
		}
	}

	protected override void Attack()
	{
		if (countAttackDelay > 0) return;

		// A Ű�� ������ ��
		if (Input.GetKeyDown(KeyCode.A))
		{
			// ���� ������ �ʱ�ȭ
			countAttackDelay = attackDelay;
			
			// �ִϸ��̼� ���
			animator.SetTrigger("Attack");

			// ����� ���
			audioSource.clip = attackSound;
			audioSource.time = 0.3f;
			audioSource.Play();

			// ���� ���� ����, ���� ���� ���� ������Ʈ�� �ִٸ� boxCast ������ ����
			RaycastHit2D boxCast = Physics2D.BoxCast(transform.position, new Vector2(1, 3f), 0, spriteRenderer.flipX ? Vector2.left : Vector2.right, attackRange, LayerMask.GetMask("Enemy"));
			// boxCast�� ����� ���� �ִٸ�, ���� �ǰݵ� ������ �Ǵ�. ������ �˹� �� �������� ����
			if (boxCast.transform != null)
			{
				combo.Intactly();
				boxCast.collider.GetComponent<Enemy>().BeShot(damage);
			}
		}
	}

	/// <summary>
	/// [������� ����] ��� ��, �ٷ� �� Scene���� ���� ����
	/// </summary>
	protected override void DeathSound()
	{

	}

	// ������Ʈ ���� ��, End Scene���� �̵�
	private void OnDestroy()
	{
		SceneManager.LoadScene("End");
	}

	// ������Ʈ�� ����� ��, �ߵ��ϴ� �Լ�
	private void OnTriggerEnter2D(Collider2D collision)
	{
		// �ٴڿ� ��Ҵ��� Ȯ���Ͽ� ���� ���θ� üũ
		if (collision.CompareTag("Ground"))
		{
			canJump = true;
		}

		// ���ÿ� ������ ü�� ���� �� �˹�
		if (collision.CompareTag("Thorn"))
		{
			BeShot(5);
		}

		// �Ѿ˿� �ǰ� ��, �ǰ� ���� �Ѿ� ���� �� ü�� ����, �˹�
		if (collision.CompareTag("Bullet"))
		{
			BeShot(collision.GetComponent<Bullet>().damage);
			Destroy(collision.gameObject);
		}
	}

}