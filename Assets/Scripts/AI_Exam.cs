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
}
