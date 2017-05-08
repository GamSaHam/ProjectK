using UnityEngine;
using System.Collections.Generic;
public class AstarMap : MonoBehaviour
{
	public List<Point> _close_map_list = new List<Point>();

    public static AstarMap _instance;

    private void Awake()
    {
        if (_instance != null)
        {
			Debug.LogError("More than one AstarMap in scene");
        }
			
        _instance = this;
    }

    public static AstarMap getInstance()
    {
        return _instance;
    }

	public void closeMap(int x,int y)
	{
		_close_map_list.Add (new Point(x * 2 , y * 2 ));
		_close_map_list.Add (new Point(x * 2 + 1, y * 2));
		_close_map_list.Add (new Point(x * 2, y * 2+1));
		_close_map_list.Add (new Point(x * 2+1, y * 2+1));
	}


}