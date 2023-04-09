using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
	public static int score = 0;

	[SerializeField] private TextMeshProUGUI text;

	private void Start()
	{
		score = 0;
	}

	private void Update()
	{
		text.text = "score : " + score;
	}
}