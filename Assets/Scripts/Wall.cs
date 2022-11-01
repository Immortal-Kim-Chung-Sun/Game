using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Wall : MonoBehaviour
{
	[SerializeField]
	UnityEvent nextStage;

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.transform.position.x - transform.position.x > 0)
		{
			nextStage.Invoke();
		}
	}
}
