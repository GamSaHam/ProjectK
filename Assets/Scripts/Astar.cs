using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Collections;

public enum MAP_STATE
{
    START_POINT = 1,
    OPEN = 2,
    CLOSE = 3,
    END_POINT = 4,
    SEARCH = 5,
    BLOCK  = 6
}

public class Point
{
	public int _x;
	public int _y;

	public Point(int x, int y)
	{
		_x = x;
		_y = y;
	}

}

public class Astar : MonoBehaviour
{
    const int MAX_MAP_SIZE = 100;
    const float MAP_FIX_SIZE = 49.5f;
	UnitStaus _unit_status;

    class Map
    {
        public int _x;
        public int _y;
        public int _g;
        public int _h;
        public int _f;
        public int _level;

        public Map(int x, int y, int level,int g,int h = 0)
        {
            _x = x;
            _y = y;
            _level = level;
            _g = g;

            _h = h;
            _f = _g + _h;
        }
    }

    public MAP_STATE[,] _map = new MAP_STATE[MAX_MAP_SIZE, MAX_MAP_SIZE];

    Thread _thread;

    public int _start_x;
    public int _start_y;

    public int _end_x;
    public int _end_y;

	public Transform _transform_target;

    private void Start()
    {
		StartCoroutine ("waitAndSeaching");
    }

	IEnumerator  waitAndSeaching()
	{

		yield return new WaitForSeconds (5f);

		_start_x = (int)Mathf.Round(transform.position.x + MAP_FIX_SIZE);
		_start_y = (int)Mathf.Round(transform.position.z + MAP_FIX_SIZE);

		_end_x =(int)Mathf.Round(_transform_target.position.x + MAP_FIX_SIZE);
		_end_y = (int)Mathf.Round(_transform_target.position.z + MAP_FIX_SIZE);

		for (int i = 0; i < MAX_MAP_SIZE; i++)
		{
			for (int j = 0; j < MAX_MAP_SIZE; j++)
			{
				_map[i, j] = MAP_STATE.OPEN;
			}
		}

		_map[_start_x, _start_y] = MAP_STATE.START_POINT;
		_map[_end_x, _end_y] = MAP_STATE.END_POINT;



		AstarMap instance = AstarMap.getInstance ();

		for (int i = 0; i < instance._close_map_list.Count; i++) {
		
			Point point = instance._close_map_list [i];

			_map [point._x, point._y] = MAP_STATE.CLOSE;

		}

		print (_map[3,5]);


		_rigidbody = GetComponent<Rigidbody>();

		_unit_status = GetComponent<UnitStaus> ();

		startSearching();

	}



    void startSearching()
    {
		if(_thread != null)
		_thread.Abort ();
        ThreadStart th = new ThreadStart(searching);
        _thread = new Thread(th);
        _thread.Start();

    }

    List<Map> _point_map_list = new List<Map>();



    List<Point> _search_map_list = new List<Point>();

	public void reSearching()
	{
		_start_x = (int)Mathf.Round(transform.position.x + MAP_FIX_SIZE);
		_start_y = (int)Mathf.Round(transform.position.z + MAP_FIX_SIZE);

		_end_x =(int)Mathf.Round(_transform_target.position.x + MAP_FIX_SIZE);
		_end_y = (int)Mathf.Round(_transform_target.position.z + MAP_FIX_SIZE);

		for (int i = 0; i < MAX_MAP_SIZE; i++)
		{
			for (int j = 0; j < MAX_MAP_SIZE; j++)
			{
				_map[i, j] = MAP_STATE.OPEN;
			}
		}

		_map[_start_x, _start_y] = MAP_STATE.START_POINT;
		_map[_end_x, _end_y] = MAP_STATE.END_POINT;

		_rigidbody = GetComponent<Rigidbody>();

		_open_map_list.Clear ();
		_point_map_list.Clear ();
		_search_map_list.Clear ();

		startSearching();

	}

    void searching()
    {
        int x = _start_x;
        int y = _start_y;
        int level = 0;

        _point_map_list.Add(new Map(x, y, level,0));

        for (int k = 0; k < 1000; k++)
        {
            int distance = Mathf.Abs(_end_x - _point_map_list[_point_map_list.Count-1]._x) + Mathf.Abs(_end_y - _point_map_list[_point_map_list.Count-1]._y);
            if (distance == 1)
                break;


            findOpenMap(ref level);
         
            level = level + 1;
            findLowCostOpenListMap(level);
        }


        _search_map_list.Add(new Point(_end_x, _end_y));

        int temp_level = -1;

	

        for (int i = _point_map_list.Count-1; i > 0; i--)
        {
            Map map = _point_map_list[i];
           
            if (temp_level == -1)
            {
                _search_map_list.Add(new Point(map._x, map._y));
                temp_level = map._level - 1;
                continue;
            }

            if (temp_level == map._level)
            {
                if (Mathf.Abs(map._x - _search_map_list[_search_map_list.Count - 1]._x) <= 1)
                {
                    if (Mathf.Abs(map._y - _search_map_list[_search_map_list.Count - 1]._y) <= 1)
                    {
                        _search_map_list.Add(new Point(map._x, map._y));
						temp_level = map._level - 1;
                    }
                }
            }
        }

        _search_map_list.Add(new Point(_start_x, _start_y));
        _search_map_list.Reverse();

        doSearching();
    }


    void findOpenMap(ref int level)
    {
        bool is_open = false;
        for (int i = 0; i < _point_map_list.Count; i++)
        {
            Map map = _point_map_list[i];
            if(map._level == level)
            {
                   is_open = findLowCostMap(map._x,map._y,map._g,level) || is_open;
             
            }
        }

        if (is_open == false)
        {
            for (int i = 0; i < _point_map_list.Count; i++)
            {
                Map map = _point_map_list[i];
                if (map._level == level)
                {
                    _map[map._x, map._y] = MAP_STATE.BLOCK;
                    _point_map_list.RemoveAt(i);

                    int index = _open_map_list.FindIndex(m => m._x == map._x || m._y == map._y);
                    _open_map_list.RemoveAt(index);
                }
            }

            level = level - 1;
        }
    }

    bool caculateCost(int x, int y, int g,int level)
    {
        bool is_open = false;

		if (AstarMap.getInstance().isCollideWithOther(x- MAP_FIX_SIZE, y- MAP_FIX_SIZE, _unit_status._radius))
        {
            return is_open;
        }

		if (x < 0 || x > MAX_MAP_SIZE - 1) 
		{
			return is_open;
		}

		if (y < 0 || y > MAX_MAP_SIZE - 1) 
		{
			return is_open;
		}

        if (_map[x, y] == MAP_STATE.OPEN)
        {
            int h = 10*(Mathf.Abs(_end_x - x) + Mathf.Abs(_end_y - y));
            _open_map_list.Add(new Map(x, y, level, g, h));
            _map[x, y] = MAP_STATE.SEARCH;
            is_open = true;
        }

        return is_open;
    }

    List<Map> _open_map_list = new List<Map>();

    bool findLowCostMap(int x,int y,int g,int level)
    {
        bool is_open = false;

        is_open = caculateCost(x + 1, y    , g + 10, level) || is_open;
        is_open = caculateCost(x    , y - 1, g + 10, level) || is_open;
        is_open = caculateCost(x - 1, y    , g + 10, level) || is_open;
        is_open = caculateCost(x    , y + 1, g + 10, level) || is_open;

        is_open =  caculateCost(x + 1, y - 1, g + 14, level) || is_open;
        is_open =  caculateCost(x - 1, y - 1, g + 14, level) || is_open;
        is_open =  caculateCost(x - 1, y + 1, g + 14, level) || is_open;
        is_open =  caculateCost(x + 1, y + 1, g + 14, level) || is_open;

        return is_open;
    }

    void findLowCostOpenListMap(int level)
    {
        _open_map_list.Sort(delegate (Map m1, Map m2)
        {
            int result = decimal.Compare(m2._level, m1._level);
            if (result == 0)
                result = decimal.Compare(m1._f, m2._f);
            return result;
        });

        int min_f = _open_map_list[0]._f;
      
        for (int i = 1; i < _open_map_list.Count; i++)
        {
            Map map = _open_map_list[i];
            if (min_f == map._f)
            {
                
				if(level-1 == map._level)
                    _point_map_list.Add(new Map(map._x, map._y, level, map._g, map._h));
            }
        }

		if(level-1 == _open_map_list[0]._level)
        _point_map_list.Add(new Map(_open_map_list[0]._x, _open_map_list[0]._y, level, _open_map_list[0]._g, _open_map_list[0]._h));
    }

    public float speed = 2;
    public Vector3[] waypoints;
    int _currentWaypoint = 0;

    Rigidbody _rigidbody;

    public bool _is_searching = false;

    public void doSearching()
    {
        _is_searching = true;
		_currentWaypoint = 0;

        waypoints = new Vector3[_search_map_list.Count];

        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = new Vector3(_search_map_list[i]._x- MAP_FIX_SIZE, 0, _search_map_list[i]._y- MAP_FIX_SIZE);
        }
    }

    void FixedUpdate()
    {
        if (_is_searching == false)
            return;

        Vector3 pos = _rigidbody.position;

        Vector3 target = waypoints[_currentWaypoint];
        target.y = pos.y;

        pos = Vector3.MoveTowards(pos, target, speed * Time.deltaTime);
        if ((pos - target).sqrMagnitude < 0.4f)
        {

            _currentWaypoint = _currentWaypoint + 1;
            if (_currentWaypoint >= waypoints.Length)
            {
                _is_searching = false;
				_rigidbody.velocity = Vector3.zero;
                return;
            }
        }
        else
            _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, Quaternion.LookRotation(target - pos, Vector3.up), speed * Time.deltaTime);

        _rigidbody.position = pos;
    }

    void OnDrawGizmosSelected()
    {
        if (waypoints == null)
            return;

        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Length; ++i)
        {
            Gizmos.DrawSphere(waypoints[i], 0.5f);
            Gizmos.DrawLine(waypoints[i], waypoints[(i + 1) % waypoints.Length]);
        }
    }


}
