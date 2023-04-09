using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
	[HideInInspector]
	public string _name = "ฐ๘น้";

	[HideInInspector]
	public long point = 0;

	public void GetData(TextMeshProUGUI textMeshProUGUI)
	{
		_name ??= textMeshProUGUI.text;
		point = Score.score;

		SendStart();
	}

	public void SendStart()
	{
		if (!CheckInternet.internetConnect) return;

		//Convert JsonString
		string json = JsonUtility.ToJson(new UserData(_name, point));

		//request Post
		StartCoroutine(Upload("http://10.80.162.73:8080/user/save", json));
	}

	IEnumerator Upload(string URL, string json)
	{
		using (UnityWebRequest request = UnityWebRequest.Post(URL, json))
		{
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
			request.uploadHandler = new UploadHandlerRaw(jsonToSend);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");

			yield return request.SendWebRequest();

			//if (request.isNetworkError || request.isHttpError)
			//{
			//	Debug.Log(request.error);
			//}
			//else
			//{
			//	Debug.Log(request.downloadHandler.text);
			//}
		}

		SceneManager.LoadScene("Start");
	}
}


