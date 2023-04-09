using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlankText : MonoBehaviour
{
	[SerializeField] private GameObject text;

	private void Start()
	{
		StartCoroutine(JobBlank());
	}

	private IEnumerator JobBlank()
	{
		while (true)
		{
			text.SetActive(false);
			yield return new WaitForSeconds(0.5f);

			text.SetActive(true);
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			SceneManager.LoadScene("Fight");
		}
	}
}
