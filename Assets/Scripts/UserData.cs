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