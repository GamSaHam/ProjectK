using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Pathfinding.Examples;
public class AI : MonoBehaviour {

    public Vector3 _way_point;

    float _turn_speed = 5f;

	public enum AI_STATE
	{
		SEARCH = 1,
		CHASE = 2,
		FIGHTING = 3

	}

	public AI_STATE _ai_state;

	

	void Awake()
	{
		_ai_state = AI_STATE.SEARCH;
        _way_point = _target.position;
    }



    public Transform _target;

    public float time = 0;

	void FixedUpdate()
	{
		UnitManager unit_manager = UnitManager.getInstance();
        BuildingManager building_manager = BuildingManager.getInstance();
        if (_ai_state == AI_STATE.SEARCH) 
		{
            if(!GetComponent<UnitStaus>()._is_air)
                GetComponent<MineBotAI>().canMove = true;

            // 유닛 발견
            bool is_collied = unit_manager.isColliedWithEnemy (gameObject,GetComponent<UnitStaus>()._search_erea);

            if (is_collied)
            {
                time = 0;
                _ai_state = AI_STATE.CHASE;


                GameObject closet_object = unit_manager.getClosetObject(gameObject);

                if (closet_object)
                {
                    _target.position = closet_object.transform.position;

                    if (!GetComponent<UnitStaus>()._is_air)
                    {

                        GetComponent<MineBotAI>().Start();
                    }
                    else
                        GetComponent<AirSearch>().StartSearch(_target.position);
                }
                return;
            }
            else
            {
                time += Time.deltaTime;

                if (time > 2f)
                {
                    time = 0;
                    _target.position = _way_point;

                    if (!GetComponent<UnitStaus>()._is_air)
                    {

                        GetComponent<MineBotAI>().SearchPath();
                    }
                    else
                        GetComponent<AirSearch>().StartSearch(_target.position);
                }
            }




            return;
		}

		if (_ai_state == AI_STATE.CHASE)
        {
		
			EventManager.getInstance ().removeEvent (gameObject);

			bool is_collied = unit_manager.isColliedWithEnemy (gameObject,GetComponent<UnitStaus>()._search_erea);

			if (!is_collied) 
			{
				_ai_state = AI_STATE.SEARCH;
			}


			GameObject collied_object = unit_manager.getColliedWithEnemy(gameObject,GetComponent<UnitStaus>()._attack_erea);

			if (collied_object != null) 
			{
                time = 0;
                EventManager.getInstance ().removeEvent (gameObject);
                // Astar stop
                if (!GetComponent<UnitStaus>()._is_air)
                    GetComponent<MineBotAI>().canMove = false;
          
                else
                    GetComponent<AirSearch>().StopSearch();
                _ai_state = AI_STATE.FIGHTING;
				int hash_code = GetComponent<AnimationController> ()._animator.GetHashCode ();
                
                 EventManager.getInstance().addAttackEvent(new EventManager.AttackEvent(doAttack), gameObject, collied_object, hash_code);
            
                
                GetComponent<AnimationController>().triggerAttack();
                
            } 
			else 
			{
                time += Time.deltaTime;

                if (time > 1.5f)
                {
                    time = 0;
                    if (!GetComponent<UnitStaus>()._is_air)
                        GetComponent<MineBotAI>().canMove = true;


                    GameObject closet_object = unit_manager.getClosetObject(gameObject);

                    if (closet_object)
                    {
                        _target.position = closet_object.transform.position;

                        if (!GetComponent<UnitStaus>()._is_air)
                            GetComponent<MineBotAI>().SearchPath();

                        else
                            GetComponent<AirSearch>().StartSearch(_target.position);
                    }
                }


            }
            return;
        }
			
		if (_ai_state == AI_STATE.FIGHTING) 
		{

			GameObject collied_object = unit_manager.getColliedWithEnemy(gameObject,GetComponent<UnitStaus>()._attack_erea);

			if (collied_object == null) 
			{

                
                _ai_state = AI_STATE.SEARCH;
			} else 
			{
                if (!GetComponent<UnitStaus>()._is_air)
                {


                    Vector3 dir = collied_object.transform.position - transform.position;
                    Quaternion look_rotation = Quaternion.LookRotation(dir);
                    Vector3 rotation = Quaternion.Lerp(transform.rotation, look_rotation, Time.deltaTime * _turn_speed).eulerAngles;
                    transform.rotation = Quaternion.Euler(rotation);
                }
                else
                {
                    Vector3 dir = collied_object.transform.position - transform.position;
                    Quaternion lookRotation = Quaternion.LookRotation(dir);
                    Vector3 rotation = lookRotation.eulerAngles;
                    transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

                }
            

            }

            return;
        }
	
	}

	void Action_Summon(GameObject target)
	{
		string prefab_path = "Prefabs/Magic1";

		if (GetComponent<UnitStaus> ()._attack_type == "summon") 
		{
			prefab_path = "Prefabs/Unit/Un_Warrier4";
		}

		GameObject prefab = Resources.Load (prefab_path) as GameObject;
		// Resources/Prefabs/Bullet.prefab 로드
		Transform[] ts = GetComponentsInChildren<Transform>();

		Vector3 position = new Vector3(transform.position.x + 1.4f,transform.position.y,transform.position.z);


		prefab.GetComponent<UnitStaus> ()._is_enemy = GetComponent<UnitStaus> ()._is_enemy;
		GameObject summon = Instantiate (prefab,position,transform.rotation);

		summon.transform.parent = GameObject.Find ("Units").GetComponent<Transform> ();
	}

	void Action_Shoot(GameObject target)
	{
		string prefab_path = "Prefabs/Magic1";
		int t_skill_id = -1;


		if (GetComponent<UnitStaus> ()._attack_type == "magic") {
			prefab_path = "Prefabs/Magic1";
		}

		if (GetComponent<UnitStaus> ()._attack_type == "arrow") {
			prefab_path = "Prefabs/Arrow1";
		}

		if (GetComponent<UnitStaus> ()._attack_type == "magic2") {
			prefab_path = "Prefabs/explosion1";
		}

		string pattern = "throw_";
		if (Regex.IsMatch (GetComponent<UnitStaus> ()._attack_type, pattern)) 
		{
			string strTmp = Regex.Replace(GetComponent<UnitStaus> ()._attack_type, @"\D", ""); 
			t_skill_id = int.Parse(strTmp);

			prefab_path = "Prefabs/MagicIce";
		}

		GameObject prefab = Resources.Load (prefab_path) as GameObject;
		// Resources/Prefabs/Bullet.prefab 로드
		Transform[] ts = GetComponentsInChildren<Transform>();

		Vector3 position = new Vector3(transform.position.x + 1.4f,transform.position.y,transform.position.z);

		foreach (Transform t in ts) 
		{
			if (t.gameObject.name == "Hand_Right_jnt") {
				position = t.gameObject.transform.position;
		
				break;
			}
		}

		if (GetComponent<UnitStaus> ()._attack_type == "magic2") 
		{
			position = target.transform.position;
		}

		Quaternion q = transform.rotation;

		GameObject bulletGO = Instantiate (prefab,position,q);


		object[] data = new object[3];
		data [0] = gameObject;
		data [1] = target;

		if (t_skill_id != -1) {
			data [2] = t_skill_id;
		}
			

		bulletGO.SendMessage ("setTarget", data);
	}

	void Action_heal(GameObject target)
	{
		
		target.GetComponent<UnitStaus> ()._current_hp = target.GetComponent<UnitStaus> ()._current_hp + GetComponent<UnitStaus> ()._attack_damage;
	
		if (target.GetComponent<UnitStaus> ()._current_hp > target.GetComponent<UnitStaus> ()._hp) {
		
			target.GetComponent<UnitStaus> ()._current_hp = target.GetComponent<UnitStaus> ()._hp;
		}

	}


	void Action_speed(GameObject target,int t_skill_id)
	{
		target.GetComponent<UnitStaus>().AddSkill(gameObject,t_skill_id);
	} 

	void Action_boom()
	{
		string prefab_path = "Prefabs/explosion2";
	
		GameObject prefab = Resources.Load (prefab_path) as GameObject;
		Transform[] ts = GetComponentsInChildren<Transform>();
		Vector3 position = new Vector3(transform.position.x,transform.position.y,transform.position.z);
		Quaternion q = transform.rotation;
		GameObject bulletGO = Instantiate (prefab,position,q);

		object[] data = new object[3];
		data [0] = gameObject;
	
		bulletGO.SendMessage ("setTarget", data);

		UnitManager.getInstance ().removeUnit (gameObject);
	}

	int _sword_man = 0;
	void doAttack(object ob)
	{
		GameObject target = (GameObject)ob;
		


		if (!UnitManager.getInstance ().isAlive (target)) {
			_ai_state = AI_STATE.SEARCH;
			EventManager.getInstance ().removeEvent (gameObject);
			return;
		}


		if(GetComponent<UnitStaus>()._attack_type == "normal")
			target.GetComponent<UnitStaus> ()._current_hp -= GetComponent<UnitStaus> ()._attack_damage;

		if (GetComponent<UnitStaus> ()._attack_type == "magic") {
			Action_Shoot (target);
		}

		if (GetComponent<UnitStaus> ()._attack_type == "arrow") {
			Action_Shoot (target);
		}

		if (GetComponent<UnitStaus> ()._attack_type == "magic2") {
			Action_Shoot (target);
		}

		string pattern = "throw_";
		if (Regex.IsMatch (GetComponent<UnitStaus> ()._attack_type, pattern)) 
		{
			Action_Shoot (target);
		}

		if (GetComponent<UnitStaus> ()._attack_type == "heal") {
			Action_heal (target);
		}

		if (GetComponent<UnitStaus> ()._attack_type == "summon") {
		
			Action_Summon (target);
		}
		if (GetComponent<UnitStaus> ()._attack_type == "boom") {
			Action_boom ();
		}



		string Pattern = "skill_";
		if (Regex.IsMatch (GetComponent<UnitStaus> ()._attack_type, Pattern)) 
		{
			string strTmp = Regex.Replace(GetComponent<UnitStaus> ()._attack_type, @"\D", ""); 
			int t_skill_id = int.Parse(strTmp);

			Action_speed (target,t_skill_id);
		}
			
		if (target.GetComponent<UnitStaus> ()._current_hp <= 0) {
			if (GetComponent<UnitStaus> ()._attack_type == "normal") 
			{
				UnitManager.getInstance ().removeUnit (target);
				EventManager.getInstance ().removeEvent (gameObject);
				_ai_state = AI_STATE.SEARCH;

			}
		} else 
		{
			if (GetComponent<UnitStaus> ()._attack_type == "heal") {
				if (target.GetComponent<UnitStaus> ()._hp == target.GetComponent<UnitStaus> ()._current_hp) {
					_ai_state = AI_STATE.SEARCH;
					return;
				}
			}

			string pattern3 = "skill_";
			if (Regex.IsMatch (GetComponent<UnitStaus> ()._attack_type, pattern3)) 
			{
				string strTmp = Regex.Replace(GetComponent<UnitStaus> ()._attack_type, @"\D", ""); 
				int t_skill_id = int.Parse(strTmp);

				if (t_skill_id == 1001) 
				{
					if (target.GetComponent<UnitStaus> ()._is_skill_speed) {
						_ai_state = AI_STATE.SEARCH;
						return;
					}
				}
			}


			if (GetComponent<UnitStaus> ()._id == 1018) {
				_sword_man++;
				if (_sword_man == 2) {
					_sword_man = 0;
					GetComponent<AnimationController> ()._animator.SetTrigger ("Attack1Trigger");

				}


			} else {
				GetComponent<AnimationController> ()._animator.SetTrigger ("Attack1Trigger");
			}
		}

	}



	void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, GetComponent<UnitStaus>()._search_erea);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, GetComponent<UnitStaus>()._attack_erea);


	}
}
