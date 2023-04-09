using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Phase : MonoBehaviour
{
	public static int count = 0;

	[Header("Cashing")]
	[SerializeField] private BoxCollider2D wallCollider;
	[SerializeField] private Transform enemySpawner;
	public UnityEvent fazeIsEnd = new UnityEvent();
	[SerializeField] private TextMeshProUGUI phaseText;

	public bool slaneEnemy = true;
	
	private Transform playerTransform;

	private void Awake()
	{
		count = 0;

		playerTransform = FindObjectOfType<Player>().transform;
	}

	private void Update()
	{
		phaseText.text = "ÆäÀÌÁî : " + (count + 1);
	}


	public void StageEnd()
	{
		wallCollider.isTrigger = true;
		Camera.main.transform.parent = playerTransform;
		
		if (count + 1 >= 5)
		{
			EndGame();
		}
	}

	public void NextStage()
	{
		count++;

		wallCollider.isTrigger = false;
		Camera.main.transform.parent = null;
		Camera.main.transform.position = new Vector3(count * 18.1f, 0, -10);
		if (count == 1)
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