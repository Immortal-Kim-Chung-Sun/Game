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

	/// <summary>
	/// 적 생성
	/// </summary>
	public void MakeEnemy()
	{
		// 적 생성 코루틴 호출
		StartCoroutine(Spawn());
	}

	/// <summary>
	/// 적 생성 후 적이 모두 처치되면 페이즈 종료
	/// </summary>
	/// <returns></returns>
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