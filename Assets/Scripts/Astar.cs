using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Collections;

public class MySolver<TPathNode, TUserContext> : SpatialAStar<TPathNode, TUserContext> where TPathNode : IPathNode<TUserContext>
{
	protected override double Heuristic(PathNode inStart, PathNode inEnd)
	{
		return Mathf.Abs(inStart.X - inEnd.X) + Mathf.Abs(inStart.Y - inEnd.Y);
	}

	protected override double NeighborDistance(PathNode inStart, PathNode inEnd)
	{
		return Heuristic(inStart, inEnd);
	}

	public MySolver(TPathNode[,] inGrid)
		: base(inGrid)
	{
	}
}

public class MyPathNode : IPathNode<Object>
{
	public int X { get; set; }
	public int Y { get; set; }
	public bool IsWall {get; set;}

	public bool IsWalkable(Object unused)
	{
		return !IsWall;
	}
}

public class EntryPoint
{
	Point _point;
	bool _is_arrived;
	public EntryPoint(Point point, bool is_arraived)
	{
		_point = point;
		_is_arrived = is_arraived;

	}

}

public class Astar : MonoBehaviour
{
	public const float k_fix_size = 49.5f;

	const int grid_width = 100;
	const int grid_height = 100;

	MyPathNode[,] grid = new MyPathNode[grid_width, grid_height];
	MySolver<MyPathNode, Object> aStar;
	UnitStaus _unit_status;

	List<Point> _search_map_list = new List<Point>();

	AnimationController _animation_controller;

	void Awake()
	{
		_rigidbody = GetComponent<Rigidbody> ();
		_unit_status = GetComponent<UnitStaus> ();

		_target = new Vector3 (_target_transform.position.x, _target_transform.position.y, _target_transform.position.z);
		_animation_controller = GetComponent<AnimationController> ();

	
	}
		
	public Transform _target_transform;
	public Vector3 _target;



	public void changeTarget(Vector3 target)
	{
		_target = target;
	}

	Thread _t;

	int _start_x;
	int _start_y;

	public void startSearching()
	{

		_start_x = (int)Mathf.Round(transform.position.x + k_fix_size);
		_start_y = (int)Mathf.Round(transform.position.z + k_fix_size);


		if (_t != null)
			_t.Abort ();

		ThreadStart th = new ThreadStart(executeAstar);
		_t = new Thread(th);
		_t.Start ();

	
	
	}


	public void executeAstar()
	{
		print ("executeAstar");
		_search_map_list.Clear ();

		for(int i=0;i<grid_width;i++)
		{
			for (int j = 0; j < grid_height; j++) 
			{
				grid[i,j ] = new MyPathNode()
				{
					IsWall = false,
					X = i,
					Y = j,
				};
			}
		}

		int start_x = _start_x;
		int start_y = _start_y;

		int end_x =(int)Mathf.Round(_target.x + k_fix_size);
		int end_y = (int)Mathf.Round(_target.z + k_fix_size);

		bool is_existed_building = false;

		for (int i = 0; i < AstarMap.getInstance ()._close_map_list.Count; i++) 
		{
			Point point = AstarMap.getInstance ()._close_map_list [i];

			if (point.X == end_x && point.Y == end_y) 
			{
				is_existed_building = true;
			
			}

			grid [point.X, point.Y].IsWall = true;
		}

		if (is_existed_building) 
		{	
			List<Point> open_list = BuildingManager.getInstance ().getPointList (new Point(end_x,end_y));

			for (int i = 0; i < open_list.Count; i++) 
			{
				Point p = open_list [i];
				grid [p.X, p.Y].IsWall = false;
			}

		}




		aStar = new MySolver<MyPathNode, Object>(grid);



		IEnumerable<MyPathNode> path  = aStar.Search (new Point (start_x, start_y), new Point (end_x, end_y), null,_unit_status._radius,false);

		if (path == null) 
		{
			path = aStar.Search (new Point (start_x, start_y), new Point (end_x, end_y), null,_unit_status._radius,true);
		}

		foreach (MyPathNode node in path)
		{
			_search_map_list.Add (new Point (node.X, node.Y));
		}

		doSearching ();
	}

    public Vector3[] waypoints;
    int _currentWaypoint = 0;
    Rigidbody _rigidbody;

    public bool _is_searching = false;

    public void doSearching()
    {
		_animation_controller.SetState (AnimationController.ANIMATION_STATE.RUN);

        _is_searching = true;
		_currentWaypoint = 0;

	
        waypoints = new Vector3[_search_map_list.Count];

        for (int i = 0; i < waypoints.Length; i++)
        {
			waypoints[i] = new Vector3(_search_map_list[i].X- k_fix_size, 0, _search_map_list[i].Y- k_fix_size);
        }
    }

    void FixedUpdate()
    {
		if (_is_searching == false) 
		{
			_rigidbody.velocity = Vector3.zero;
			return;

		}
        Vector3 pos = _rigidbody.position;

        Vector3 target = waypoints[_currentWaypoint];
        target.y = pos.y;

		pos = Vector3.MoveTowards(pos, target, _unit_status._speed * Time.deltaTime);
        if ((pos - target).sqrMagnitude < 0.4f)
        {
            _currentWaypoint = _currentWaypoint + 1;
            if (_currentWaypoint >= waypoints.Length)
            {
                _is_searching = false;
				_rigidbody.velocity = Vector3.zero;
				GetComponent<AnimationController> ().SetState (AnimationController.ANIMATION_STATE.IDLE);
                return;
            }
        }
        else
			_rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, Quaternion.LookRotation(target - pos, Vector3.up), _unit_status._speed * Time.deltaTime);

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
