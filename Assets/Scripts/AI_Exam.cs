using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Exam : MonoBehaviour {

	public Astar[] _astar;

	public void doSearch()
	{
		for (int i = 0; i < _astar.Length; i++) {
			Astar astar = _astar [i];
			astar.reSearching ();
		
		}
	}

	public AstarMap _astarmap;

	public void mapDisable()
	{

		_astarmap._close_map_list.Add (new Point (3, 5));
		_astarmap._close_map_list.Add (new Point (3, 4));
		_astarmap._close_map_list.Add (new Point (3, 3));




	}

}
