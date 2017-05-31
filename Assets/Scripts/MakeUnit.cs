using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding.Examples;
public class MakeUnit : MonoBehaviour
{

    public float _time = 0;
    public int _unit_id = 0;

	// Use this for initialization
	void Start () {
        StartCoroutine("Make");
	}

    IEnumerator Make()
    {
        while (true)
        {
            yield return new WaitForSeconds(_time);

            bool is_enemy = GetComponent<UnitStaus>()._is_enemy;

            Vector3 enemy_position = Vector3.zero;
            if (is_enemy)
                enemy_position = GameObject.Find("Target1").transform.position;
            else
                enemy_position = GameObject.Find("Target2").transform.position;

            string prefab_path = "Prefabs/Target";
            GameObject prefab1 = Resources.Load(prefab_path) as GameObject;
            GameObject target = Instantiate(prefab1, enemy_position, transform.rotation);

            if(_unit_id == 1001)
                prefab_path = "Prefabs/Unit/Un_Warrier";
            else if(_unit_id == 1002)
                prefab_path = "Prefabs/Unit/Un_Archer";
            if (_unit_id == 1003)
                prefab_path = "Prefabs/Unit/Un_Magic";

            if (_unit_id == 1012)
                prefab_path = "Prefabs/Unit/Un_Warrier3";
            if (_unit_id == 1017)
                prefab_path = "Prefabs/Unit/Un_Boom";
            if (_unit_id == 1020)
                prefab_path = "Prefabs/Unit/Un_Warrier5";

            if (_unit_id == 1014)
                prefab_path = "Prefabs/Unit/Un_Warrier2";

            if (_unit_id == 1010)
                prefab_path = "Prefabs/Unit/Un_Magic_poison";
            if (_unit_id == 1012)
                prefab_path = "Prefabs/Unit/Un_Animal2";



            GameObject prefab = Resources.Load(prefab_path) as GameObject;
            prefab.GetComponent<MineBotAI>().target = target.transform;
            prefab.GetComponent<AI>()._target = target.transform;


            Vector3 make_position = UnitManager.getInstance().getMakePosition(transform.position, GetComponent<UnitStaus>()._radius, prefab.GetComponent<UnitStaus>()._radius);
            Vector3 position = make_position;


            prefab.GetComponent<UnitStaus>()._is_enemy = GetComponent<UnitStaus>()._is_enemy;
            GameObject summon = Instantiate(prefab, position, transform.rotation);
            summon.transform.parent = GameObject.Find("Units").GetComponent<Transform>();

            summon.GetComponent<MineBotAI>().SearchPath();
            UnitManager.getInstance().addUnit(summon);
        }

    }
	// Update is called once per frame
	void Update () {
		
	}
}
