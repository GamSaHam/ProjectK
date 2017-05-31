using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Arrow : MonoBehaviour {

	public  Transform bullet;   // 포물체

	private float tx;
	private float ty;
	private float tz;
	private float v;
	public float g = 9.8f;

	private float elapsed_time;
	public float max_height = 2;

	private float t;

	private Vector3 start_pos;
	private Vector3 end_pos;

	public float dat;  //도착점 도달 시간 

	public GameObject _origin;
	public GameObject _target;

	void Start()
	{
		act = HitTarget;
		start_pos = _origin.transform.position;
		end_pos = _target.transform.position;

		Shoot (transform, start_pos, end_pos, g, max_height,act);
	}

	System.Action act;



	public void Shoot(Transform bullet, Vector3 startPos, Vector3 endPos, float g, float max_height, System.Action onComplete)
	{

		start_pos = startPos;

		end_pos = endPos;

		this.g = g;

		this.max_height = max_height;

		this.bullet = bullet;

		this.bullet.position = start_pos;



		var dh = endPos.y - startPos.y;

		var mh = max_height - startPos.y;

		ty = Mathf.Sqrt(2 * this.g * mh);



		float a = this.g;

		float b = -2 * ty;

		float c = 2 * dh;



		dat = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);



		tx = -(startPos.x - endPos.x) / dat;

		tz = -(startPos.z - endPos.z) / dat;



		this.elapsed_time = 0;

		StartCoroutine(this.ShootImpl(onComplete));
	}

	IEnumerator ShootImpl(System.Action onComplete)
	{
		while (true)
		{

			this.elapsed_time += Time.deltaTime;

			var tx = start_pos.x + this.tx * elapsed_time;
			var ty = start_pos.y + this.ty * elapsed_time - 0.5f * g * elapsed_time * elapsed_time;
			var tz = start_pos.z + this.tz * elapsed_time;


			var tpos = new Vector3(tx, ty, tz);

			bullet.transform.LookAt(tpos);
			bullet.transform.position = tpos;

			if (this.elapsed_time >= this.dat)
				break;

			yield return null;
		}

		onComplete();
	}





	public void setTarget(object[] data)
	{
		_origin =(GameObject)data [0];
		_target =(GameObject)data [1];


	}
		
	// Update is called once per frame
	void Update () 
	{
		
		if (_target == null) 
		{
			Destroy (gameObject);
			return;
		}
			
	}



	void HitTarget()
	{
		

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

		if (_target.GetComponent<UnitStaus> ()._current_hp <= 0) {
			UnitManager.getInstance ().removeUnit (_target);
			_origin.GetComponent<AI> ()._ai_state = AI.AI_STATE.CHASE;
			EventManager.getInstance ().removeEvent (_origin);
		}


		StartCoroutine ("deleteObject", (object)0.5f);
	}

	IEnumerator deleteObject(object obj)
	{

		float time = (float)obj;
		yield return new WaitForSeconds (time);
		Destroy (gameObject);

	}

}
