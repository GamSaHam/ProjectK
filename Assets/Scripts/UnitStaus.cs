using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStaus : MonoBehaviour {

	public int _id =  0;

	public float _radius = 0;

	void Start()
	{
		
		for (int i = 0; i < TableUnit.getInstance().data.Count; i++)
		{
			if (_id == (int)TableUnit.getInstance().data [i] ["id"]) 
			{
				_radius = (float)TableUnit.getInstance().data [i] ["radius"];
			}
		}
	}
}
