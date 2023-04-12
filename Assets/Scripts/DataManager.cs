using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
	/// <summary>
	/// 이름과 점수를 저장하는 데이터 클래스
	/// </summary>
	private UserData userData;

	/// <summary>
	/// UserData 클래스에 이름과 점수 저장 후 SendStart 호출
	/// </summary>
	/// <param name="textMeshProUGUI"></param>
	public void GetData(TextMeshProUGUI textMeshProUGUI)
	{
		userData = new UserData(textMeshProUGUI.text, Score.score);

		SendStart();
	}

	public void SendStart()
	{
		// 인터넷이 연결되어있지 않다면 본 함수 실행 종료
		if (!CheckInternet.internetConnect) return;

		// 데이터 클래스 Json으로 변환
		string json = JsonUtility.ToJson(userData);

		// URL 주소 설정 후 서버에 Request Post
		StartCoroutine(Upload("http://172.20.10.4:8080/user/save", json));
	}

	/// <summary>
	/// 서버에 전송 이후 처음 타이틀 화면으로 이동
	/// </summary>
	/// <param name="URL"></param>
	/// <param name="json"></param>
	/// <returns></returns>
	private IEnumerator Upload(string URL, string json)
	{
		// UnityWebRequest를 통해 http 통신
		// 지정된 URL에 byte 단위로 전송
		using (UnityWebRequest request = UnityWebRequest.Post(URL, json))
		{
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
			request.uploadHandler = new UploadHandlerRaw(jsonToSend);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");

			// 코루틴으로 프레임 마다 차례차례 전송
			yield return request.SendWebRequest();

			// 유니티 에디터 일 때 로그 출력
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

		// 타이틀 화면으로 이동
		SceneManager.LoadScene("Start");
	}
}