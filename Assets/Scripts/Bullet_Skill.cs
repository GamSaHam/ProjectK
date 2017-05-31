using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Skill : MonoBehaviour {
	public GameObject _origin;
	public GameObject _target;
	public int _t_skill_id;

	public float speed = 10f;

	public void setTarget(object[] data)
	{
		_origin =(GameObject)data [0];
		_target =(GameObject)data [1];
		_t_skill_id =(int)data [2];
	}


	// Update is called once per frame
	void Update () 
	{
		if (_target == null) 
		{
			Destroy (gameObject);
			return;
		}

		Vector3 dir = _target.transform.position - transform.position;

		float distanceThisFrame = speed * Time.deltaTime;

		if (dir.magnitude <= distanceThisFrame) {
			HitTarget ();
			return;
		}

		transform.Translate (dir.normalized * distanceThisFrame,Space.World);

	}
	bool is_hit = false;



	void HitTarget()
	{
		if (is_hit)
			return;

		if (!UnitManager.getInstance ().isAlive (_origin)) 
		{
			StartCoroutine ("deleteObject", (object)0.5f);
			return;

		}
		   if (!UnitManager.getInstance().isAlive(_target))
        {
            StartCoroutine("deleteObject", (object)0.5f);
            return;

        }

		_target.GetComponent<UnitStaus> ()._current_hp -= _origin.GetComponent<UnitStaus> ()._attack_damage;
        if(_target.GetComponent<UnitStaus>()._is_building == false)
            _target.GetComponent<UnitStaus>().AddSkill(null,_t_skill_id);

		if (_target.GetComponent<UnitStaus> ()._current_hp <= 0) {
			UnitManager.getInstance ().removeUnit (_target);
			_origin.GetComponent<AI> ()._ai_state = AI.AI_STATE.CHASE;
			EventManager.getInstance ().removeEvent (_origin);
		}




		StartCoroutine ("deleteObject", (object)0.5f);
	}

	IEnumerator deleteObject(object obj)
	{
		is_hit = true;

		float time = (float)obj;
		yield return new WaitForSeconds (time);

        Destroy(gameObject);


    }
}
