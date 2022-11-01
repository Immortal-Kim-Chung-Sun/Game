using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[SerializeField]
	private Phase phase;

	public EnemyData[] enemyDatas = new EnemyData[5];

	[SerializeField]
	private GameObject[] enemys = new GameObject[4];

	private void Start()
	{
		MakeEnemy();
	}

	public void MakeEnemy()
	{
		StartCoroutine(Spawn());
	}

	private IEnumerator Spawn()
	{
		if (Phase.phase is 0) yield return new WaitForSeconds(2); 

		for (int i = 0; i < enemyDatas[Phase.phase].enemy1; i++)
		{
			Instantiate(enemys[0], transform.position, Quaternion.identity);
			yield return new WaitForSeconds(1);
		}

		for (int i = 0; i < enemyDatas[Phase.phase].enemy2; i++)
		{
			Instantiate(enemys[1], transform.position, Quaternion.identity);
			yield return new WaitForSeconds(1);
		}

		for (int i = 0; i < enemyDatas[Phase.phase].enemy3; i++)
		{
			Instantiate(enemys[2], transform.position, Quaternion.identity);
			yield return new WaitForSeconds(1);
		}

		while (FindObjectsOfType<Enemy>().Length is not 0)
		{
			yield return null;
		}
		
		phase.fazeIsEnd.Invoke();
	}
}

[System.Serializable]
public class EnemyData
{
	public int enemy1 = 0;
	public int enemy2 = 0;
	public int enemy3 = 0;
}