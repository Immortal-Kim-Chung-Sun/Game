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
	/// �� ����
	/// </summary>
	public void MakeEnemy()
	{
		// �� ���� �ڷ�ƾ ȣ��
		StartCoroutine(Spawn());
	}

	/// <summary>
	/// �� ���� �� ���� ��� óġ�Ǹ� ������ ����
	/// </summary>
	/// <returns></returns>
	private IEnumerator Spawn()
	{
		// �� ó�� �������� 2�� ���
		if (Phase.count == 0) yield return new WaitForSeconds(2);

		// Enemy1, Enemy2, Boss ������� 1�ʸ��� ����
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

		// Phase Ŭ������ ������ ���� �̺�Ʈ ȣ��
		phase.fazeIsEnd.Invoke();
	}
}