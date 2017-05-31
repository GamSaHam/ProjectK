using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour 
{

	public static EffectManager _instance;

	private void Awake()
	{

		if (_instance != null)
		{
			Debug.LogError("More than one EffectManager in scene");
		}

		_instance = this;
	}

	public static EffectManager getInstance()
	{
		return _instance;
	}


	List<Effect> _effect_list  = new List<Effect>();

	class Effect
	{
		public GameObject _target;
		public float _time;
		public float _current_time;
		public int _t_skill_id;
		public Effect(GameObject target,float time,int t_skill_id)
		{
			
			_target = target;
			_time = time;
			_current_time = 0;
			_t_skill_id = t_skill_id;
		}
	}


	public void addEffect(GameObject target,int t_skill_id)
	{
		float time = TableSkill.getInstance ().getTime (t_skill_id);

		if (time == -1)
			return;


		for (int i = 0; i < _effect_list.Count; i++) {
			Effect effect = _effect_list [i];

			if (effect._t_skill_id == t_skill_id) {
				if (effect._target == target) {
					_effect_list.RemoveAt (i);
				}
			}
		}

		_effect_list.Add (new Effect(target,time,t_skill_id));

	}

	float position_time = 0;

	public void changeColor(GameObject obj,float r,float g,float b,float a)
	{ 
		ChangeMaterial[] change_materials = obj.GetComponentsInChildren<ChangeMaterial> ();
		foreach (ChangeMaterial change_material in change_materials) {
			change_material.setColor (r, g, b, a);
		}
	}

	void FixedUpdate()
	{
		for (int i = 0; i < _effect_list.Count; i++) 
		{
			Effect effect = _effect_list [i];
			effect._current_time += Time.deltaTime;

			if(effect._t_skill_id == 1001)
			{
				float up_rate = TableSkill.getInstance ().getUpRate (effect._t_skill_id);
				effect._target.GetComponent<UnitStaus> ()._current_speed = effect._target.GetComponent<UnitStaus> ()._speed + up_rate;
			}

			if (effect._t_skill_id == 1002) 
			{
				float up_rate = TableSkill.getInstance ().getUpRate (effect._t_skill_id);
				effect._target.GetComponent<UnitStaus> ()._current_speed = effect._target.GetComponent<UnitStaus> ()._speed - up_rate;
				effect._target.GetComponent<AnimationController> ()._animator.speed = 1.0f - up_rate;
				changeColor (effect._target, 0.2f, 0.2f, 1, 1);
			}

			if (effect._t_skill_id == 1003) 
			{
				position_time += Time.deltaTime;

				if(position_time >= 1f)
				{
					float up_rate = TableSkill.getInstance ().getUpRate (effect._t_skill_id);
					if(effect._target.GetComponent<UnitStaus> ()._current_hp > 0)
						effect._target.GetComponent<UnitStaus> ()._current_hp = effect._target.GetComponent<UnitStaus> ()._current_hp - up_rate;
					position_time -= 1f;	
				}
					
				changeColor (effect._target, 0.2f, 1f, 0.2f, 1);
			}
			if (effect._t_skill_id == 1004) 
			{
				float up_rate = TableSkill.getInstance ().getUpRate (effect._t_skill_id);
				effect._target.GetComponent<UnitStaus> ()._current_speed = 0;
				effect._target.GetComponent<AnimationController> ()._animator.speed = 0;
				changeColor (effect._target, 1f, 1f, 0.2f, 1);
			}


			if (effect._current_time >= effect._time) 
			{
				effect._target.GetComponent<UnitStaus> ().removeSkill (effect._t_skill_id);
				if (effect._t_skill_id == 1001) 
				{
					effect._target.GetComponent<UnitStaus> ()._current_speed = effect._target.GetComponent<UnitStaus> ()._speed;
				}
				if (effect._t_skill_id == 1002) 
				{
					effect._target.GetComponent<UnitStaus> ()._current_speed = effect._target.GetComponent<UnitStaus> ()._speed;
					effect._target.GetComponent<AnimationController> ()._animator.speed = 1.0f;

					changeColor (effect._target, 1f, 1f, 1f, 1f);

				}
				if (effect._t_skill_id == 1003) 
				{
					position_time = 0;
					changeColor (effect._target, 1f, 1f, 1f, 1f);
				}
				if (effect._t_skill_id == 1004) 
				{
					effect._target.GetComponent<UnitStaus> ()._current_speed = effect._target.GetComponent<UnitStaus> ()._speed;
					effect._target.GetComponent<AnimationController> ()._animator.speed = 1;
					changeColor (effect._target, 1f, 1f, 1f, 1f);
				}

				_effect_list.RemoveAt (i);
			}


		
		}



	}



	public void removeEffect(GameObject target)
	{
		for (int i = 0; i < _effect_list.Count; i++) {
			Effect effect = _effect_list [i];
			if (effect._target == target) {
			
				_effect_list.RemoveAt (i);
				return;
			}
		
		}
	}



}
