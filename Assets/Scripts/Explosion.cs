using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	public GameObject _origin;
	public GameObject _target;



	float _radius = 1.5f;
	// Use this for initialization
	void Start () 
	{
		for (int i = 0; i < UnitManager.getInstance ()._unit_list.Count; i++) 
		{
			GameObject obj = UnitManager.getInstance ()._unit_list [i];

			if (_origin.GetComponent<UnitStaus> ()._is_enemy == obj.GetComponent<UnitStaus> ()._is_enemy) {
				continue;
			}

			if ( UnitManager.getInstance().isColideWithCircleToCircle (transform.position.x, transform.position.z, _radius,
				obj.transform.position.x, obj.transform.position.z, obj.GetComponent<UnitStaus> ()._radius)) {
			
				obj.GetComponent<UnitStaus> ()._current_hp -= _origin.GetComponent<UnitStaus> ()._attack_damage;

				if (obj.GetComponent<UnitStaus> ()._current_hp <= 0) {
					UnitManager.getInstance ().removeUnit (obj);
					_origin.GetComponent<AI> ()._ai_state = AI.AI_STATE.CHASE;
					EventManager.getInstance ().removeEvent (_origin);
				}
			
			}


		
		}


		StartCoroutine ("removeAfterTime", 0.5f);
	}



	IEnumerator removeAfterTime(float _time)
	{
		yield return new WaitForSeconds(_time);

		Destroy(gameObject);

	}


	public void setTarget(object[] data)
	{
		_origin =(GameObject)data [0];
		_target =(GameObject)data [1];
	}

}
