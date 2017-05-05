using UnityEngine;
using System.Collections.Generic;
public class AstarMap : MonoBehaviour
{
	public const int MAX_MAP_SIZE = 100;
	public List<Point> _close_map_list = new List<Point>();

    private Vector3[] _unit_position;
	private UnitStaus[] _unit_status;

    public static AstarMap _instance;



	public GameObject[] _unit;

    private void Awake()
    {
        if (_instance != null)
        {
			Debug.LogError("More than one AstarMap in scene");
        }
			
        _instance = this;

	

    }
		

    private void FixedUpdate()
    {
		_unit_position = new Vector3[_unit.Length];
		_unit_status = new UnitStaus[_unit.Length];
		for (int i = 0; i < _unit.Length; i++)
        {
			_unit_position[i] = new Vector3(_unit[i].transform.position.x,0, _unit[i].transform.position.z);
			_unit_status [i] = _unit [i].GetComponent<UnitStaus> ();
        }
    }

    public static AstarMap getInstance()
    {
        return _instance;
    }

    bool isColideWithCircleToCircle(float x1,float y1, float r1, float x2, float y2, float r2)
    {
        float delta_x = x1 - x2;
        float delta_y = y1 - y2;

        float length = Mathf.Sqrt(delta_x * delta_x + delta_y * delta_y);

        if (length > (r1 + r2))
        {
            return false;
        }

        return true;
    }

    public bool isCollideWithOther(float x, float y, float radius )
    {
		if (_unit_position == null)
			return false;

        for (int i = 0; i < _unit_position.Length; i++)
        {
          
            Vector2 _pos = new Vector2(_unit_position[i].x,_unit_position[i].z);
            
            
			if (isColideWithCircleToCircle(x, y, radius, _pos.x, _pos.y, _unit_status[i]._radius))
            {
                return true;
            }
        }

        return false;
    }

	public void closeMap(int x,int y)
	{
		_close_map_list.Add (new Point(x * 2 , y * 2 ));
		_close_map_list.Add (new Point(x * 2 + 1, y * 2));
		_close_map_list.Add (new Point(x * 2, y * 2+1));
		_close_map_list.Add (new Point(x * 2+1, y * 2+1));

	
	}


}
