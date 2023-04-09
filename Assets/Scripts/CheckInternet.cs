using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInternet : MonoBehaviour
{
	[SerializeField] private GameObject offlineModeText;

	public static bool internetConnect = false;

	private void Start()
	{
		internetConnect = Application.internetReachability != NetworkReachability.NotReachable;
		offlineModeText.SetActive(!internetConnect);
	}
}
