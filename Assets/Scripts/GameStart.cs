using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
		StartCoroutine(Blank());
    }

	IEnumerator Blank()
	{
		while (true)
		{
			text.gameObject.SetActive(false);
			yield return new WaitForSeconds(0.5f);

			text.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.5f);
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetMouseButtonDown(0))
		{
			SceneManager.LoadScene("Fight");
		}
    }
}
