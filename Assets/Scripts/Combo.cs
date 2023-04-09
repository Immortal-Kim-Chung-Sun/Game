using System.Collections;
using TMPro;
using UnityEngine;

public class Combo : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI text;

	private int combo = 0;

	private float countTime = 0f;

	private void Update()
	{
		countTime += Time.deltaTime;

		if (countTime > 1.5f)
		{
			text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
			combo = 0;
		}

		text.text = combo.ToString();
	}

	public void Intactly()
	{
		text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
		StartCoroutine(SetSize());
		countTime = 0;
		combo++;
	}

	private IEnumerator SetSize()
	{
		for (int count = 0; count < 20; count++)
		{
			text.fontSize++;
			yield return null;
		}

		for (int count = 0; count < 20; count++)
		{
			text.fontSize--;
			yield return null;
		}
	}
}
