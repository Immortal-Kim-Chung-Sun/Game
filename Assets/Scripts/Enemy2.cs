using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Enemy
{
	private AudioSource _audio;

	[SerializeField]
	private GameObject bullet;

	private bool shooting = false;

	private void Start()
	{
		_audio = GameObject.Find("Enemy2Audio").GetComponent<AudioSource>();

		// 뮤트: true일 경우 소리가 나지 않음
		_audio.mute = false;

		// 루핑: true일 경우 반복 재생
		_audio.loop = false;

		// 자동 재생: true일 경우 자동 재생
		_audio.playOnAwake = false;
	}
	//보스 만들면 끝!!!
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

		StartCoroutine(Wait());
	}

	private IEnumerator Wait()
	{
		shooting = true;

		animator.SetBool("Attack", true);

		_audio.clip = _audioClip[1];
		_audio.time = 0f;
		_audio.Play();

		yield return new WaitForSeconds(0.5f);
		Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, spriteRenderer.flipX ? 0 : 180));
		
		animator.SetBool("Attack", false);

		shooting = false;
		canAttack = false;
	}

	protected override void Move()
	{
		if (shooting) return;

		float range = playerTransform.position.x - transform.position.x;
		canAttack = Mathf.Abs(range) <= attackRange;
		spriteRenderer.flipX = range > 0;
		transform.position += new Vector3(spriteRenderer.flipX ? Time.deltaTime : -Time.deltaTime, 0) * 1.5f;
	}
}
