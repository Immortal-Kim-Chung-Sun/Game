using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Phase : MonoBehaviour
{
	public static int phase = 0;

	[SerializeField]
	private Camera _camera;

	[SerializeField]
	private Transform playerTransform;

	[SerializeField]
	private BoxCollider2D wallCollider;

	[SerializeField]
	private Transform enemySpawner;

	public UnityEvent fazeIsEnd = new UnityEvent();

	[SerializeField]
	private TextMeshProUGUI textMesh;

	public bool slaneEnemy = true;

	private void Awake()
	{
		phase = 0;
	}

	private void Update()
	{
		if (slaneEnemy) return;

		textMesh.text = "ÆäÀÌÁî : " + (phase + 1);
	}


	public void StageEnd()
	{
		wallCollider.isTrigger = true;
		slaneEnemy = true;
		_camera.transform.parent = playerTransform;
		phase++;
		if (phase >= 5)
		{
			EndGame();
		}
	}

	public void NextStage()
	{
		wallCollider.isTrigger = false;
		slaneEnemy = false;
		_camera.transform.parent = null;
		_camera.transform.position = new Vector3(phase * 18.1f, 0, -10);
		if (phase is 1)
		{
			wallCollider.transform.parent.position = new Vector3(17, 0);
		}
		else
		{
			wallCollider.transform.parent.position += new Vector3(18.1f, 0);
		}
		enemySpawner.position += new Vector3(18.1f, 0);
	}

	private void EndGame()
	{
		SceneManager.LoadScene("End");
	}
}