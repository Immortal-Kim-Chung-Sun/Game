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
&nbsp;&nbsp;&nbsp;&nbsp;● 피격, 피격 시 넉백 기능이 있습니다.
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
&nbsp;&nbsp;&nbsp;&nbsp;● 이동, 공격, 점프, 체력 UI 표현 등의 기능이 있습니다.
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

<b>Enemy</b><br>
&nbsp;&nbsp;&nbsp;&nbsp;● 적 스크립트의 기틀이 되는 부모 클래스입니다.<br>
&nbsp;&nbsp;&nbsp;&nbsp;● 플레이어 좌표 값, 획득 가능한 점수, 공격 여부 등 적에게 필요한 변수 값과 사망 시 사운드 재생 기능이 있습니다.
<details>
    <summary><i>자세한 코드</i></summary>
    
  ```C#
using UnityEngine;

public abstract class Enemy : Actor
{
	// 적 사망 시 플레이어가 획득하는 점수
	[SerializeField] private int score;

	[Header("Cashing")]
	// 적 사운드 저장 클래스
	[SerializeField] protected EnemySound enemySound;

	// 적의 공격 여부
	protected bool canAttack = false;

	protected Transform playerTransform;

	protected override void Awake()
	{
		base.Awake();

		playerTransform = FindObjectOfType<Player>().transform;
	}

	private void OnDestroy()
	{
		// 씬이 이동되면 플레이어가 적을 삭제하지 않아도 알아서 적이 삭제된다.
		// 따라서 플레이어가 있을 때, 즉 게임이 진행 중일 때만 점수를 획득하게 조건문을 달았다.
		if (playerTransform != null)
		{
			Score.score += score;
		}
	}

	protected override void DeathSound()
	{
		audioSource.clip = enemySound.death;
		audioSource.time = 0.5f;
		audioSource.Play();
	}
}

  ```
</details><br>

<b>Enemy1</b><br>
&nbsp;&nbsp;&nbsp;&nbsp;● 보스를 포함한 3마리 적 중 하나의 .<br>
&nbsp;&nbsp;&nbsp;&nbsp;● 모든 적은 플레이어를 따라오는 AI가 탑재되어 있습니다.<br>
&nbsp;&nbsp;&nbsp;&nbsp;● Enemy1은 칼로 근접공격을 합니다.<br>
&nbsp;&nbsp;&nbsp;&nbsp;● 이동 AI, 공격 기능이 있습니다.
<details>
    <summary><i>자세한 코드</i></summary>
    
  ```C#
using UnityEngine;

// 추상화 클래스 Enemy 상속
public class Enemy1 : Enemy
{
	/// RayCast로 판정 내에 플레이어가 있는지 확인 후 공격
	protected override void Attack()
	{
		// 범위를 벗어났다면 함수 종료
		if (!canAttack) return;

		// 딜레이가 남아있다면 함수 종료
		if (countAttackDelay > 0) return;

		// 공격 시작. 딜레이 초기화
		countAttackDelay = attackDelay;

		// 애니메이션 출력
		animator.SetTrigger("Attack");

		// 사운드 출력
		audioSource.PlayOneShot(enemySound.attack[0]);

		// RayCast로 공격 판정 생성. 공격 판정 내에 플레이어가 있다면 boxCast에 값 저장
		RaycastHit2D boxCast = Physics2D.BoxCast(transform.position, new Vector2(1f, 2f), 0, spriteRenderer.flipX ? Vector2.right : Vector2.left, attackRange, LayerMask.GetMask("Player"));
		// boxCast에 값이 저장되어있다면, 플레이어에게 넉백 및 데미지를 가함
		if (boxCast.transform != null)
		{
			boxCast.collider.GetComponent<Player>().BeShot(damage);
		}
	}

	/// 플레이어를 따라가는 AI를 기반으로 이동
	protected override void Move()
	{
		float range = playerTransform.position.x - transform.position.x;
		// 플레이어와의 거리가 공격할 수 있는 거리인지 확인 후 Bool 변수에 저장
		canAttack = Mathf.Abs(range) <= attackRange;
		// 플레이어의 위치에 따라 이미지 방향 전환
		spriteRenderer.flipX = range > 0;
		// 플레이어를 향해 이동
		transform.position += new Vector3(spriteRenderer.flipX ? Time.deltaTime : -Time.deltaTime, 0) * speed;
	}
}
  ```
</details><br>

<b>DataManager</b><br>
&nbsp;&nbsp;&nbsp;&nbsp;● 입력한 이름과 게임을 플레이하면서 얻은 점수를 서버에 전송하는 클래스입니다.<br>
&nbsp;&nbsp;&nbsp;&nbsp;● 이름과 점수 데이터 클래스에 저장, 데이터 클래스 Json 변환, 지정한 URL에 Json 전송 기능이 있습니다.
<details>
    <summary><i>자세한 코드</i></summary>
    
  ```C#
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
	// 이름과 점수를 저장하는 데이터 클래스
	private UserData userData;

	// UserData 클래스에 이름과 점수 저장 후 SendStart 호출
	public void GetData(TextMeshProUGUI textMeshProUGUI)
	{
		userData = new UserData(textMeshProUGUI.text, Score.score);

		SendStart();
	}

	public void SendStart()
	{
		// 인터넷이 연결되어있지 않다면 본 함수 실행 종료
		if (!CheckInternet.internetConnect) return;

		// 데이터 클래스 Json으로 변환
		string json = JsonUtility.ToJson(userData);

		// URL 주소 설정 후 서버에 Request Post
		StartCoroutine(Upload("http://10.80.162.73:8080/user/save", json));
	}

	// 서버에 전송 이후 처음 타이틀 화면으로 이동
	private IEnumerator Upload(string URL, string json)
	{
		// UnityWebRequest를 통해 http 통신
		// 지정된 URL에 byte 단위로 전송
		using (UnityWebRequest request = UnityWebRequest.Post(URL, json))
		{
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
			request.uploadHandler = new UploadHandlerRaw(jsonToSend);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");

			// 코루틴으로 프레임 마다 차례차례 전송
			yield return request.SendWebRequest();

			// 유니티 에디터 일 때 로그 출력
#if UNITY_EDITOR
			if (request.isNetworkError || request.isHttpError)
			{
				Debug.Log(request.error);
			}
			else
			{
				Debug.Log(request.downloadHandler.text);
			}
#endif
		}

		// 타이틀 화면으로 이동
		SceneManager.LoadScene("Start");
	}
}
  ```
</details><br>

<b>Phase</b><br>
&nbsp;&nbsp;&nbsp;&nbsp;● 코루틴과 UnityEvent를 이용하여 게임 페이즈(스테이지)를 관리하는 클래스입니다.<br>
&nbsp;&nbsp;&nbsp;&nbsp;● 페이즈 종료, 다음 페이즈 시작, 페이즈 모두 클리어 시 끝 Scene으로 가기 기능이 있습니다.
<details>
    <summary><i>자세한 코드</i></summary>
    
  ```C#
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Phase : MonoBehaviour
{
	// 현재 페이즈
	public static int count = 0;

	[Header("Cashing")]
	[SerializeField] private BoxCollider2D wallCollider;
	[SerializeField] private Transform enemySpawnerTransform;
	// UnityEvent로 변수 내에 StageEnd 함수를 지정해놓아 fazeIsEnd를 Invoke하면 StageEnd 함수가 실행됨
	// 페이즈 종료 이벤트 변수
	public UnityEvent fazeIsEnd = new UnityEvent();
	[SerializeField] private TextMeshProUGUI phaseText;
	
	private Transform playerTransform;

	private void Awake()
	{
		count = 0;

		playerTransform = FindObjectOfType<Player>().transform;
	}

	private void Update()
	{
		// 현재 페이즈를 텍스트로 UI에 출력
		phaseText.text = "페이즈 : " + (count + 1);
	}

	// 페이즈 종료
	public void StageEnd()
	{
		// 벽 판정 비활성화
		wallCollider.isTrigger = true;
		// 카메라가 플레이어를 따라가도록 설정
		Camera.main.transform.parent = playerTransform;
		
		// 종료한 페이즈가 5번째라면 끝 Scene으로 이동
		if (count + 1 >= 5)
		{
			EndGame();
		}
	}

	// 페이즈 시작
	public void NextStage()
	{
		// 페이즈 1 증가
		count++;

		// 벽 판정 생성
		wallCollider.isTrigger = false;
		// 카메라가 더 이상 플레이어를 따라가지 않게 설정
		Camera.main.transform.parent = null;
		Camera.main.transform.position = new Vector3(count * 18.1f, 0, -10);
		// 벽 판정을 현재 페이즈 위치로 이동
		if (count == 1)
		{
			wallCollider.transform.parent.position = new Vector3(17, 0);
		}
		else
		{
			wallCollider.transform.parent.position += new Vector3(18.1f, 0);
		}
		// 적 생성기를 현재 페이즈 위치로 이동
		enemySpawnerTransform.position += new Vector3(18.1f, 0);
	}

	// 끝 Scene으로 이동
	private void EndGame()
	{
		SceneManager.LoadScene("End");
	}
}
  ```
</details><br>

<b>EnemySpawner</b><br>
&nbsp;&nbsp;&nbsp;&nbsp;● 코루틴을 이용하여 적을 생성하는 클래스입니다.<br>
&nbsp;&nbsp;&nbsp;&nbsp;● 적 생성 후 적이 모두 처치되면 UnityEvent로 Phase의 페이즈 종료 함수 호출 기능이 있습니다.
<details>
    <summary><i>자세한 코드</i></summary>
    
  ```C#
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[Header("Cashing")]
	[SerializeField] private Phase phase;
	public EnemyData[] enemyDatas = new EnemyData[5];
	[SerializeField] private GameObject[] enemys = new GameObject[4];

	private void Start()
	{
		MakeEnemy();
	}

	// 적 생성
	public void MakeEnemy()
	{
		// 적 생성 코루틴 호출
		StartCoroutine(Spawn());
	}

	// 적 생성 후 적이 모두 처치되면 페이즈 종료
	private IEnumerator Spawn()
	{
		// 맨 처음 페이즈라면 2초 대기
		if (Phase.count == 0) yield return new WaitForSeconds(2);

		// Enemy1, Enemy2, Boss 순서대로 1초마다 생성
		for (int i = 0; i < enemyDatas[Phase.count].enemy1; i++)
		{
			Instantiate(enemys[0], transform.position, Quaternion.identity);
			yield return new WaitForSeconds(1);
		}

		for (int i = 0; i < enemyDatas[Phase.count].enemy2; i++)
		{
			Instantiate(enemys[1], transform.position, Quaternion.identity);
			yield return new WaitForSeconds(1);
		}

		for (int i = 0; i < enemyDatas[Phase.count].boss; i++)
		{
			Instantiate(enemys[2], transform.position, Quaternion.identity);
			yield return new WaitForSeconds(1);
		}

		while (FindObjectsOfType<Enemy>().Length != 0)
		{
			yield return null;
		}

		// Phase 클래스의 페이즈 종료 이벤트 호출
		phase.fazeIsEnd.Invoke();
	}
}
  ```
</details>
