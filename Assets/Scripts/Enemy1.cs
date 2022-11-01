using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Enemy
{
	private AudioSource _audio;

	private void Start()
	{
		attackDelay = 1f;
		
		_audio = GameObject.Find("Enemy1Audio").GetComponent<AudioSource>();

		// ��Ʈ: true�� ��� �Ҹ��� ���� ����
		_audio.mute = false;

		// ����: true�� ��� �ݺ� ���
		_audio.loop = false;

		// �ڵ� ���: true�� ��� �ڵ� ���
		_audio.playOnAwake = false;
	}

	protected override void DeathSound()
	{
		_audio.clip = _audioClip[0];
		_audio.time = 0.5f;
		_audio.Play();
	}

	protected override void Attack()
	{
		base.Attack();

		if (!canAttack) return;

		animator.SetTrigger("Attack");

		_audio.clip = _audioClip[1];
		_audio.time = 0f;
		_audio.Play();

		RaycastHit2D boxCast = Physics2D.BoxCast(transform.position, new Vector2(1f, 2f), 0, spriteRenderer.flipX ? Vector2.right : Vector2.left, attackRange, LayerMask.GetMask("Player"));
		if (boxCast.transform != null)
		{
			boxCast.collider.GetComponent<Player>().BeAttacked = true;
			boxCast.collider.GetComponent<Player>().hp -= 10;
		}
	}

	protected override void Move()
	{
		float range = playerTransform.position.x - transform.position.x;
		canAttack = Mathf.Abs(range) <= attackRange;
		spriteRenderer.flipX = range > 0;
		transform.position += new Vector3(spriteRenderer.flipX ? Time.deltaTime : -Time.deltaTime, 0) * 1.5f;
	}
}
