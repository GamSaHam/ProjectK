using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStaus : MonoBehaviour {

	public int _id =  0;
	public float _radius = 0;
	public float _speed = 0;
	public float _attack_erea = 0;

	public string _attack_type;
	public bool _is_enemy = false;
	public float _attack_damage = 0;
	public float _hp = 0;
	public float _current_hp = 0;
	void Start()
	{
		
		for (int i = 0; i < TableUnit.getInstance().data.Count; i++)
		{
			Dictionary<string, object> dic = TableUnit.getInstance ().data [i];

			int id = (int)dic["id"];

			if (_id == id) 
			{
				
				_radius = (float)dic["radius"];
				_speed = (float)dic["speed"];
				_attack_erea = (float)dic["attack_erea"];
				_attack_type = (string)dic["attack_type"];
				_attack_damage = (float)dic ["attack_damage"];
				_hp = (float)dic ["hp"];

			}
		}

		_current_hp = _hp;
	}
}
