using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Actor
{
	public bool BeAttacked
	{
		get { return beAttacked; }
		set { beAttacked = value; }
	}

	[SerializeField]
	private AudioClip attackSound;

	[SerializeField]
	private Image hpBar;

	[SerializeField]
	private TextMeshProUGUI textMeshProUGUI;

	[SerializeField]
	private Combo combo;

	[SerializeField]
	private float jumpRange;

	public bool canJump = false;

	private AudioSource _audio;

	protected override void Awake()
	{
		base.Awake();

		hp = 300;

		animator = GetComponent<Animator>();

		_audio = GameObject.Find("Main Camera").GetComponent<AudioSource>();
	}

	protected override void Update()
	{
		animator.SetBool("Walk", false);

		base.Update();

		Jump();

		hpBar.fillAmount = hp / 300f;

		textMeshProUGUI.text = hp + "/300";
	}

	protected override void Move()
	{
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			spriteRenderer.flipX = true;
			transform.Translate(Vector2.left * Time.deltaTime * 5);

			animator.SetBool("Walk", true);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			spriteRenderer.flipX = false;
			transform.Translate(Vector2.right * Time.deltaTime * 5);

			animator.SetBool("Walk", true);
		}
	}

	private void Jump()
	{
		if (!Input.GetKeyDown(KeyCode.UpArrow)) return;

		if (!canJump) return;

		_rigidbody2D.AddForce(new Vector2(0, jumpRange), ForceMode2D.Impulse);

		canJump = false;
	}

	protected override void Attack()
	{
		if (!Input.GetKeyDown(KeyCode.A)) return;

		animator.SetTrigger("Attack");

		_audio.clip = attackSound;
		_audio.time = 0.3f;
		_audio.Play();

		base.Attack();

		RaycastHit2D boxCast = Physics2D.BoxCast(transform.position, new Vector2(1, 3f), 0, spriteRenderer.flipX ? Vector2.left : Vector2.right, attackRange, LayerMask.GetMask("Enemy"));
		if (boxCast.transform != null)
		{
			combo.Intactly();
			boxCast.collider.GetComponent<Enemy>().BeAttacked = true;
		}
	}

	protected override IEnumerator KnockBack()
	{
		isHit = true;

		animator.SetBool("KnockBack", true);

		_rigidbody2D.AddForce(new Vector2(spriteRenderer.flipX ? knockBackRange : -knockBackRange, 0), ForceMode2D.Impulse);

		yield return new WaitForSeconds(0.2f);
		animator.SetBool("KnockBack", false);

		if (hp <= 0)
		{
			Destroy(gameObject);
		}

		isHit = false;
	}

	private void OnDestroy()
	{
		SceneManager.LoadScene("End");
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.CompareTag("Ground"))
		{
			canJump = true;
		}

		if (collision.transform.CompareTag("Bullet"))
		{
			BeAttacked = true;
			hp -= 30;
			Destroy(collision.gameObject);
		}
	}
}

