using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using FoW;

public class UnitManager : MonoBehaviour {

	public List<GameObject> _unit_list;

	private Vector3[] _unit_position;
	private UnitStaus[] _unit_status;
	public FogOfWar _fog;


	public static UnitManager _instance;

	public GameObject _unit_parant;

	private void Awake()
	{
		_unit_list = new List<GameObject> ();

		_fog = GameObject.Find("Main Camera").GetComponent<FogOfWar>();

		if (_instance != null)
		{
			Debug.LogError("More than one UnitManager in scene");
		}

		_instance = this;
	}

	private void Start()
	{
		Transform[] objects = _unit_parant.GetComponentsInChildren<Transform> ();

		foreach(Transform t in objects)
		{
			
			string Pattern = "Un_";
			if (Regex.IsMatch (t.name, Pattern)) {
				_unit_list.Add (t.gameObject);
			}

		}
	}



	public static UnitManager getInstance()
	{
		return _instance;
	}


	private void FixedUpdate()
	{
		_unit_position = new Vector3[_unit_list.Count];
		_unit_status = new UnitStaus[_unit_list.Count];
		for (int i = 0; i < _unit_list.Count; i++)
		{
			_unit_position[i] = new Vector3(_unit_list[i].transform.position.x,0, _unit_list[i].transform.position.z);
			_unit_status [i] = _unit_list [i].GetComponent<UnitStaus> ();
		}
	}




	public bool isPointInCircle(float x1,float y1,float radius,float x2, float y2)
	{
		float delta_x = x1 - x2;
		float delta_y = y1 - y2;
	
		float length = Mathf.Sqrt (delta_x * delta_x + delta_y * delta_y);

		if (length > radius) 
		{
			return false;
		}

		return true;
	}

	public bool isCollideWithOther(GameObject obj,float x, float y, float radius , float end_x, float end_y)
	{
		if (_unit_position == null)
			return false;


		for (int i = 0; i < _unit_position.Length; i++)
		{
			Vector2 _pos = new Vector2(_unit_position[i].x,_unit_position[i].z);

			if (_fog.IsInCompleteFog (_unit_position[i])) {
				continue;
			}

			if (obj == _unit_list [i])
				continue;

//			print ("x:"+x+"y:"+y+"radius"+radius);
//			print ("_pos.x:"+_pos.x+"_pos.y:"+_pos.y+"radius"+ _unit_status[i]._radius);
			if (isColideWithCircleToCircle(x, y, radius, _pos.x, _pos.y, _unit_status[i]._radius))
			{
				if (isPointInCircle (_pos.x, _pos.y, _unit_status [i]._radius, end_x, end_y)) {
					print ("continue");
					continue;
				}
					

				return true;
			}
		}

		return false;
	}

    public Vector3 getMakePosition(Vector3 position, float building_radius,float unit_radius)
    {
        Vector3 temp_position = new Vector3(position.x - building_radius - unit_radius, 2, position.z-building_radius - unit_radius);
        

        

        return temp_position;
    }



    public void addUnit(GameObject obj)
    {
        _unit_list.Add(obj);

    }

	public void removeUnit(GameObject obj)
	{
		EffectManager.getInstance ().removeEffect (obj);

		for (int i = 0; i < _unit_list.Count; i++) {
			if (_unit_list [i] == obj) {
				_unit_list.RemoveAt (i);
                
				break;
			}
		}

    
  

        if (obj.GetComponent<UnitStaus>()._is_building)
        {
            BuildingManager.getInstance().removeBuilding(obj);

        }


		Destroy (obj);
	}

	public bool isAlive(GameObject obj)
	{
		for (int i = 0; i < _unit_list.Count; i++) {
			if (_unit_list [i] == obj) {
				return true;
			
			}
		}

		return false;
	}



    public bool isColideWithCircleToCircle(float x1, float y1, float r1, float x2, float y2, float r2)
    {
        float delta_x = x1 - x2;
        float delta_y = y1 - y2;

        float length = Mathf.Sqrt(delta_x * delta_x + delta_y * delta_y);

        if (length > (r1 + r2))
        {
            return false;
        }

        return true;
    }



    public bool isSkillCondition(GameObject origin, GameObject target)
    {

        if (origin.GetComponent<UnitStaus>()._is_target_enemy)
        {
            if (origin.GetComponent<UnitStaus>()._is_enemy == target.GetComponent<UnitStaus>()._is_enemy)
                return true;
        }
        else
        {
            if (origin.GetComponent<UnitStaus>()._is_enemy != target.GetComponent<UnitStaus>()._is_enemy)
                return true;
        }


        if (origin.GetComponent<UnitStaus>()._attack_type2 == UnitStaus.ATTACK_TYPE2.GROUND && target.GetComponent<UnitStaus>()._is_air == true)
            return true;

        if (origin.GetComponent<UnitStaus>()._attack_type2 == UnitStaus.ATTACK_TYPE2.AIR && target.GetComponent<UnitStaus>()._is_air != true)
            return true;


        string pattern = "skill_";

        if (origin.GetComponent<UnitStaus>()._attack_type == "heal")
        {


            if (target.GetComponent<UnitStaus>()._hp == target.GetComponent<UnitStaus>()._current_hp)
                return true;


        }
        else if (Regex.IsMatch(origin.GetComponent<UnitStaus>()._attack_type, pattern))
        {


            string strTmp = Regex.Replace(origin.GetComponent<UnitStaus>()._attack_type, @"\D", "");
            int t_skill_id = int.Parse(strTmp);
            if (t_skill_id == 1001)
            {
                if (target.GetComponent<UnitStaus>()._is_skill_speed)
                    return true;

            }



        }

        return false;
    }


    public bool isColliedWithEnemy(GameObject origin, float radius)
    {


        for (int i = 0; i < _unit_list.Count; i++)
        {
            GameObject obj = _unit_list[i];

            if (origin == obj)
                continue;

            bool is_condition = isSkillCondition(origin, obj);
            if (is_condition)
                continue;

            if (isColideWithCircleToCircle(origin.transform.position.x, origin.transform.position.z, radius,
                obj.transform.position.x, obj.transform.position.z, obj.GetComponent<UnitStaus>()._radius))
            {
                return true;
            }


        }

        return false;
    }



    public GameObject getColliedWithEnemy( GameObject origin, float radius)
    {
        for (int i = 0; i < _unit_list.Count; i++)
        {
            GameObject obj = _unit_list[i];

            if (origin == obj)
                continue;

            bool is_condition = isSkillCondition(origin, obj);
            if (is_condition)
                continue;




            if (isColideWithCircleToCircle(origin.transform.position.x, origin.transform.position.z, radius,
                obj.transform.position.x, obj.transform.position.z, obj.GetComponent<UnitStaus>()._radius))
            {
                return _unit_list[i];
            }


        }

        return null;
    }



    public GameObject getClosetObject( GameObject origin)
    {
        float min_distance = -1f;
        int min_index = -1;

        float x = origin.transform.position.x;
        float y = origin.transform.position.z;

        for (int i = 0; i < _unit_list.Count; i++)
        {
            GameObject target = _unit_list[i];

            if (origin == target)
            {
                continue;
            }

            bool is_condition = isSkillCondition(origin, target);
            if (is_condition)
                continue;

            float x2 = _unit_list[i].transform.position.x;
            float y2 = _unit_list[i].transform.position.z;

            float distance = (float)Mathf.Sqrt(Mathf.Pow(x - x2, 2) + Mathf.Pow(y - y2, 2));

            if (min_distance == -1f)
            {
                min_distance = distance;
                min_index = i;
                continue;
            }

            if (distance < min_distance)
            {

                min_distance = distance;
                min_index = i;
            }
        }

        if (_unit_list.Count > min_index)
            return _unit_list[min_index];
        else
            return null;
        

    }

}
