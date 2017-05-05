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
	

public class Astar : MonoBehaviour
{
	public const float k_fix_size = 49.5f;

	const int grid_width = 100;
	const int grid_height = 100;

	MyPathNode[,] grid = new MyPathNode[grid_width, grid_height];


	MySolver<MyPathNode, Object> aStar;

	UnitStaus _unit_status;

	void Awake()
	{



		_rigidbody = GetComponent<Rigidbody> ();
		_unit_status = GetComponent<UnitStaus> ();
	}
		
	List<Point> _search_map_list = new List<Point>();
	// Use this for initialization

	public Transform _target;
			
	public void executeAstar()
	{
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


		for (int i = 0; i < AstarMap.getInstance ()._close_map_list.Count; i++) {
			Point point = AstarMap.getInstance ()._close_map_list [i];

			grid [point.X, point.Y].IsWall = true;

		}

		aStar = new MySolver<MyPathNode, Object>(grid);

		int start_x = (int)Mathf.Round(transform.position.x + k_fix_size);
		int start_y = (int)Mathf.Round(transform.position.z + k_fix_size);

		int end_x =(int)Mathf.Round(_target.position.x + k_fix_size);
		int end_y = (int)Mathf.Round(_target.position.z + k_fix_size);

		IEnumerable<MyPathNode> path  = aStar.Search (new Point (start_x, start_y), new Point (end_x, end_y), null,_unit_status._radius);

		foreach (MyPathNode node in path)
		{
			_search_map_list.Add (new Point (node.X, node.Y));
			//print("x:"+node.X.ToString() + " y:"+ node.Y.ToString());

		}

		doSearching ();
	
	}

  
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
			waypoints[i] = new Vector3(_search_map_list[i].X- k_fix_size, 0, _search_map_list[i].Y- k_fix_size);
        }
    }

    void FixedUpdate()
    {
        if (_is_searching == false)
            return;

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
