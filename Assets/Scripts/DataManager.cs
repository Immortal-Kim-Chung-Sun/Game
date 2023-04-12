using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
	/// <summary>
	/// �̸��� ������ �����ϴ� ������ Ŭ����
	/// </summary>
	private UserData userData;

	/// <summary>
	/// UserData Ŭ������ �̸��� ���� ���� �� SendStart ȣ��
	/// </summary>
	/// <param name="textMeshProUGUI"></param>
	public void GetData(TextMeshProUGUI textMeshProUGUI)
	{
		userData = new UserData(textMeshProUGUI.text, Score.score);

		SendStart();
	}

	public void SendStart()
	{
		// ���ͳ��� ����Ǿ����� �ʴٸ� �� �Լ� ���� ����
		if (!CheckInternet.internetConnect) return;

		// ������ Ŭ���� Json���� ��ȯ
		string json = JsonUtility.ToJson(userData);

		// URL �ּ� ���� �� ������ Request Post
		StartCoroutine(Upload("http://172.20.10.4:8080/user/save", json));
	}

	/// <summary>
	/// ������ ���� ���� ó�� Ÿ��Ʋ ȭ������ �̵�
	/// </summary>
	/// <param name="URL"></param>
	/// <param name="json"></param>
	/// <returns></returns>
	private IEnumerator Upload(string URL, string json)
	{
		// UnityWebRequest�� ���� http ���
		// ������ URL�� byte ������ ����
		using (UnityWebRequest request = UnityWebRequest.Post(URL, json))
		{
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
			request.uploadHandler = new UploadHandlerRaw(jsonToSend);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");

			// �ڷ�ƾ���� ������ ���� �������� ����
			yield return request.SendWebRequest();

			// ����Ƽ ������ �� �� �α� ���
#if UNITY_EDITOR
			if (request.isNetworkError || request.isHttpError)
			{
				Debug.Log(request.error);
			}
			else
			{
				Debug.Log(request.downloadHandler.text);
			}
#endif
		}

		// Ÿ��Ʋ ȭ������ �̵�
		SceneManager.LoadScene("Start");
	}
}