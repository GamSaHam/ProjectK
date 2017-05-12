using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoW;

public class UnitStaus : MonoBehaviour {

	public int _id =  0;
	public float _radius = 0;
	public float _speed = 0;
	public float _current_speed = 0;
	public float _attack_erea = 0;

	public string _attack_type;
	public bool _is_enemy = false;
	public float _attack_damage = 0;
	public float _hp = 0;
	public float _current_hp = 0;
	public float _search_erea = 0;

	public bool _is_skill_speed = false;

	public bool _is_air = false;

	public enum ATTACK_TYPE2
	{
		GROUND = 1,
		BOTH = 2,
		AIR = 3
	}
	public ATTACK_TYPE2 _attack_type2;

	public bool _is_target_enemy = true;

	public void readDictionary(Dictionary<string, object> dic,string key,ref float ref_value)
	{
		if (dic [key] is int) {
			int temp_speed = (int)dic [key];
			ref_value = (float)temp_speed;
		} else 
		{
			ref_value = (float)dic [key];
		}
	}

	void Start()
	{
		
		for (int i = 0; i < TableUnit.getInstance().data.Count; i++)
		{
			Dictionary<string, object> dic = TableUnit.getInstance ().data [i];

			int id = (int)dic["id"];

			if (_id == id) 
			{
				readDictionary (dic, "radius",ref _radius);
				readDictionary (dic, "speed",ref _speed);
				readDictionary (dic, "attack_erea",ref _attack_erea);
				_attack_type = (string)dic["attack_type"];
				readDictionary (dic, "attack_damage",ref _attack_damage);
				readDictionary (dic, "hp",ref _hp);
				readDictionary (dic, "search_erea",ref _search_erea);

				float attack_type2 = 0;
				readDictionary (dic, "attack_type2",ref attack_type2);

				if ((int)attack_type2 == 1)
					_attack_type2 = ATTACK_TYPE2.GROUND;
				else if((int)attack_type2 == 2)
					_attack_type2 = ATTACK_TYPE2.BOTH;
				else if((int)attack_type2 == 3)
					_attack_type2 = ATTACK_TYPE2.AIR;

				float is_air = 0;
				readDictionary (dic, "is_air",ref is_air);
				if ((int)is_air == 0) 
				{
					_is_air = false;
				} else 
				{
					_is_air = true;
				}

				float skill_target = 0;
				readDictionary (dic, "skill_target",ref skill_target);

				if ((int)skill_target == 0) 
				{
					_is_target_enemy = true;

				}else if((int)skill_target == 1)
				{
					_is_target_enemy = false;
				}

			}
		}

		_current_hp = _hp;
		_current_speed = _speed;

		if (_is_enemy == true) {
			GetComponent<FogOfWarUnit> ().enabled = false;

		} else {
			HideInFog[] hide_in_foges = GetComponentsInChildren<HideInFog> ();
			for (int i = 0; i < hide_in_foges.Length; i++) {
				hide_in_foges [i].enabled = false;

			}
		}
	}


	public void AddSkill(GameObject caster,int t_skill_id)
	{
		
		_is_skill_speed = true;
		EffectManager.getInstance ().addEffect (gameObject,t_skill_id);


	}

	public void removeSkill(int t_skill_id)
	{
		if (t_skill_id == 1001) 
		{
			_is_skill_speed = false;
		}
	}

}
