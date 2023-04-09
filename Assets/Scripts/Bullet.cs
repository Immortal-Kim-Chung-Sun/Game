using UnityEngine;

public class Bullet : MonoBehaviour
{
	[Header("Value")]
	[SerializeField] private float speed = 5;
	public int damage;

	private void Update()
	{
		transform.Translate(speed * Time.deltaTime * Vector3.right, Space.Self);
	}

	private void OnBecameInvisible()
	{
		Vector3 path = Camera.main.WorldToScreenPoint(transform.position);
		if (path.x < 0 || path.x > Screen.width || path.y < 0 || path.y > Screen.height)
		{
			Destroy(gameObject);
		}
	}
}