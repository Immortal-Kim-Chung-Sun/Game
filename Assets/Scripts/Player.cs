using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

		hpBar.fillAmount = hp / (float)maxHP;

		hpText.text = hp + "/" + maxHP;
	}

	protected override void Move()
	{
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			spriteRenderer.flipX = true;
			transform.Translate(speed * Time.deltaTime * Vector2.left);

			animator.SetBool("Walk", true);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			spriteRenderer.flipX = false;
			transform.Translate(speed * Time.deltaTime * Vector2.right);

			animator.SetBool("Walk", true);
		}
	}

	private void Jump()
	{
		if (!canJump) return;

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			rigidbody2d.AddForce(new Vector2(0, jumpRange), ForceMode2D.Impulse);
			canJump = false;
		}
	}

	protected override void Attack()
	{
		if (countAttackDelay > 0) return;


		if (Input.GetKeyDown(KeyCode.A))
		{
			countAttackDelay = attackDelay;
			
			animator.SetTrigger("Attack");

			audioSource.clip = attackSound;
			audioSource.time = 0.3f;
			audioSource.Play();

			RaycastHit2D boxCast = Physics2D.BoxCast(transform.position, new Vector2(1, 3f), 0, spriteRenderer.flipX ? Vector2.left : Vector2.right, attackRange, LayerMask.GetMask("Enemy"));
			if (boxCast.transform != null)
			{
				combo.Intactly();
				boxCast.collider.GetComponent<Enemy>().BeShot(damage);
			}
		}
	}

	protected override void DeathSound()
	{

	}

	private void OnDestroy()
	{
		SceneManager.LoadScene("End");
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Ground"))
		{
			canJump = true;
		}

		if (collision.CompareTag("Thorn"))
		{
			BeShot(5);
		}

		if (collision.CompareTag("Bullet"))
		{
			BeShot(collision.GetComponent<Bullet>().damage);
			Destroy(collision.gameObject);
		}
	}

}

