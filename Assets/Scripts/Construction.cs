using UnityEngine;
using System.Collections.Generic;

public enum BUILD_TYPE
{
    FOUR_BLOCK = 4,
    NINE_BLOCK = 9
}


public class Construction : MonoBehaviour
{


    //public GameObject _standard_turret_prefab;
	public BUILD_TYPE _building_type = BUILD_TYPE.NINE_BLOCK;

    CResources _resources;

    private void Awake()
    {
        _resources = GetComponent<CResources>();
     //   _target_to_build = Resources.Load();
    }

    private GameObject _target_to_build;

    public GameObject getTargetBuilding()
    {
        return _target_to_build;
    }

    public BUILD_TYPE getBuildingType()
    {
        return _building_type;
    }
		
    public bool _is_map_open = false;

    public float _min;
    public float _gas;


    public void setBuildType(int num)
    {
        if (num == 33)
        {
            _building_type = BUILD_TYPE.NINE_BLOCK;
        }
        //  if ((int)t_build["build_type"] == 22)
        if (num == 22)
        {
            _building_type = BUILD_TYPE.FOUR_BLOCK;
        }
    }

    public void setBuildingPath(string id)
    {
        if (id == "2001")
        {
            _target_to_build = Resources.Load("Prefabs/Building/King") as GameObject;
        }

        if (id == "2002")
        {
            _target_to_build = Resources.Load("Prefabs/Building/Farm") as GameObject;
        }
        if (id == "2003")
        {
            _target_to_build = Resources.Load("Prefabs/Building/MilitaryTent") as GameObject;
        }
        if (id == "2004")
        {
            _target_to_build = Resources.Load("Prefabs/Building/MilitaryTent2") as GameObject;
        }
        if (id == "2005")
        {
            _target_to_build = Resources.Load("Prefabs/Building/MilitaryTent3") as GameObject;
        }

        if (id == "3001")
        {
            _target_to_build = Resources.Load("Prefabs/Building/King2") as GameObject;
        }

        if (id == "3002")
        {
            _target_to_build = Resources.Load("Prefabs/Building/Farm2") as GameObject;
        }
        if (id == "3003")
        {
            _target_to_build = Resources.Load("Prefabs/Building/MilitaryTentOak") as GameObject;
        }
        if (id == "3004")
        {
            _target_to_build = Resources.Load("Prefabs/Building/MilitaryTentOak2") as GameObject;
        }
        if (id == "3005")
        {
            _target_to_build = Resources.Load("Prefabs/Building/MilitaryTentOak3") as GameObject;
        }
        if (id == "4001")
        {
            _target_to_build = Resources.Load("Prefabs/Building/King3") as GameObject;
        }
        if (id == "4002")
        {
            _target_to_build = Resources.Load("Prefabs/Building/Farm3") as GameObject;
        }
        if (id == "4003")
        {
            _target_to_build = Resources.Load("Prefabs/Building/MilitaryTentElf") as GameObject;
        }
        if (id == "4004")
        {
            _target_to_build = Resources.Load("Prefabs/Building/MilitaryTentElf2") as GameObject;
        }
        if (id == "4005")
        {
            _target_to_build = Resources.Load("Prefabs/Building/MilitaryTentElf3") as GameObject;
        }


    }

    public void mapOpen(string id)
    {
        Dictionary<string, object> t_build = TableBuilding.getInstance().getDictionaryById(int.Parse(id));

        if (t_build == null)
            return;

        Table.readDictionary(t_build, "min", ref _min);
        Table.readDictionary(t_build, "gas", ref _gas);

        int temp_min = 0;
        int temp_gas = 0;

        temp_min = _resources.getMin();
        temp_gas = _resources.getGas();

        if (MapManager.getInstance()._is_open == false)
        {
            if (_min > temp_min)
            {

                MessageManager.getInstance().popupMessage("미네랄이 부족합니다.");
                return;
            }

            if (_gas > temp_gas)
            {
                MessageManager.getInstance().popupMessage("가스가 부족합니다.");
                return;
            }
        }


        setBuildType((int)t_build["build_type"]);
        setBuildingPath(id);



        MapManager.getInstance().toogleMapsState();


    }

    // maps are not opened
    public bool startBuilding(string id,bool is_enemy)
    {

        Dictionary<string, object> t_build = TableBuilding.getInstance().getDictionaryById(int.Parse(id));

        if (t_build == null)
            return false;

        Table.readDictionary(t_build, "min", ref _min);
        Table.readDictionary(t_build, "gas", ref _gas);

        int temp_min = 0;
        int temp_gas = 0;

      
        temp_min = _resources.getMin();
        temp_gas = _resources.getGas();
        
       

        if (_min > temp_min)
        {
            if(is_enemy == false)
            MessageManager.getInstance().popupMessage("미네랄이 부족합니다.");
            return false;
        }

        if (_gas > temp_gas)
        {
            if (is_enemy == false)
                MessageManager.getInstance().popupMessage("가스가 부족합니다.");
            return false;
        }

        setBuildType((int)t_build["build_type"]);
        setBuildingPath(id);


        return true;
    }
    

    public bool isBuilable(ref Vector3 position,string obj_name,BUILD_TYPE building_type)
    {
        if (building_type == BUILD_TYPE.FOUR_BLOCK)
        {
            position = new Vector3(position.x , position.y, position.z );

            string[] map_position = obj_name.Split('p');

            int x = int.Parse(map_position[1]);
            int y = int.Parse(map_position[2]);

            if (!isMapBuildable(x + 1, y) || !isMapBuildable(x, y + 1) || !isMapBuildable(x + 1, y + 1))
            {
                //if (is_enemy == false)
                 //   MessageManager.getInstance().popupMessage("건설이 불가능 합니다.");
                return false;
            }
        }


        return true;
    }

    public void commandBuild(float pos_x,float pos_y,float pos_z,string obj_name,bool is_enemy)
    {
        GameObject _target_to_build = getTargetBuilding();
        BUILD_TYPE building_type = getBuildingType();

        Vector3 position = new Vector3(pos_x, pos_y, pos_z);

        bool is_builable = isBuilable(ref position, obj_name, building_type);
        if (is_builable == false)
        {
            if (is_enemy == false)
                MessageManager.getInstance().popupMessage("건설이 불가능 합니다.");

            return;
        }


        if (_resources.getMin() - (int)_min < 0)
        {
            MessageManager.getInstance().popupMessage("미네랄이 부족합니다.");
            return;
        }

        if (_resources.getGas() - (int)_gas < 0)
        {
            MessageManager.getInstance().popupMessage("가스가 부족합니다.");
            return;
        }
        


     
        _resources.setMin(_resources.getMin() - (int)_min);
        _resources.setGas(_resources.getGas() - (int)_gas);


        

        

 


        if (position != null)
        {
            GameObject building = Instantiate(_target_to_build, position, transform.rotation);
            building.GetComponent<UnitStaus>()._is_enemy = is_enemy;
            building.GetComponent<UnitStaus>()._is_building = true;

            string[] map_position = obj_name.Split('p');

            int x = int.Parse(map_position[1]);
            int y = int.Parse(map_position[2]);

            BuildingManager.getInstance().insertBuilding(building, x, y, building_type);
            UnitManager.getInstance().addUnit(building);
        }
    }

    bool isMapBuildable(int x, int y)
    {
        string tile_name = string.Format("SF_Env_Tile_Grass_p{0}p{1}", x, y);
        GameObject map = GameObject.Find(tile_name);

        if (map)
        {
            if (map.GetComponent<Node>()._is_builable)
                return true;

        }

        return false;
    }

  

}
