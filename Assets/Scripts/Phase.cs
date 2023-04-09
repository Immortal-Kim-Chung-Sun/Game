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
	/// <summary>
	/// 페이즈 종료 이벤트 변수
	/// </summary>
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

	/// <summary>
	/// 페이즈 종료
	/// </summary>
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

	/// <summary>
	/// 페이즈 시작
	/// </summary>
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

	/// <summary>
	/// 끝 Scene으로 이동
	/// </summary>
	private void EndGame()
	{
		SceneManager.LoadScene("End");
	}
}