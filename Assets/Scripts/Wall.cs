using UnityEngine;
using UnityEngine.Events;

public class Wall : MonoBehaviour
{
	[SerializeField] private UnityEvent nextStage;

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!collision.CompareTag("Player")) return;

		if (collision.transform.position.x - transform.position.x > 0)
		{
			nextStage.Invoke();
		}
	}
}
