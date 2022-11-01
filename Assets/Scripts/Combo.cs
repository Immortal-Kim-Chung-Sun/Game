using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Combo : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI textMesh;
	[HideInInspector] public int combo = 0;
	private float countTime = 0f;


	private void Update()
    {
		countTime += Time.deltaTime;

		if (countTime > 1.5f)
		{
			textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0);
			combo = 0;
		}

		textMesh.text = combo.ToString();
    }

	public void Intactly()
	{
		textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1);
		StartCoroutine(SetSize());
		countTime = 0;
		combo++;
	}

	private IEnumerator SetSize()
	{
		for (int i = 0; i < 20; i++)
		{
			textMesh.fontSize++;
			yield return null;
		}

		for (int i = 0; i < 20; i++)
		{
			textMesh.fontSize--;
			yield return null;
		}
	}
}
