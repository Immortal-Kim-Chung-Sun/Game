using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SendData : MonoBehaviour
{
	[HideInInspector]
	public string _name = "½Å»ç¶û";

	[HideInInspector]
	public long point = 0;

	public void GetData(TextMeshProUGUI text)
	{
		_name = text.text;
		point = Score.score;

		print(_name);
		print(point);

		SendStart();
	}

	public void SendStart()
	{
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
			request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");

			yield return request.SendWebRequest();

			if (request.isNetworkError || request.isHttpError)
			{
				Debug.Log(request.error);
			}
			else
			{
				Debug.Log(request.downloadHandler.text);
			}
		}

		SceneManager.LoadScene("Start");
	}
}

[System.Serializable]
public class UserData
{
	public string name;

	public long point;
	public UserData(string name, long point)
	{
		this.name = name;
		this.point = point;
	}
}
