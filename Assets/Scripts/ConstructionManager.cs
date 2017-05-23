using UnityEngine;
using System.Collections.Generic;



public class ConstructionManager : MonoBehaviour
{
    List<Dictionary<string, object>> data;


    public static ConstructionManager _instance;

    private void Awake()
    {
        data = CSVReader.Read("CSV/table_building");

        if (_instance != null)
        {
            Debug.LogError("More than one buildManager in scene");
        }

        _instance = this;
    }

    public static ConstructionManager getInstance()
    {
        return _instance;
    }

    public enum BUILD_TYPE
    {
        FOUR_BLOCK = 4,
		NINE_BLOCK = 9
    }

    //public GameObject _standard_turret_prefab;
	public BUILD_TYPE _building_type = BUILD_TYPE.NINE_BLOCK;


    private void Start()
    {
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



    public void readDictionary(Dictionary<string, object> dic, string key, ref float ref_value)
    {
        if (dic[key] is int)
        {
            int temp_speed = (int)dic[key];
            ref_value = (float)temp_speed;
        }
        else
        {
            ref_value = (float)dic[key];
        }
    }

    public CResources _resources1;
    public CResources _resources2;

    

    public void mapOpen(string id)
    {
        Dictionary<string, object> t_build = findMapFromId(id);

        if (t_build == null)
            return;

        readDictionary(t_build, "min", ref _min);
        readDictionary(t_build, "gas", ref _gas);

        int temp_min = 0;
        int temp_gas = 0;

        temp_min = _resources1.getMin();
        temp_gas = _resources1.getGas();
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

        

        if ((int)t_build["build_type"] == 33)
        {
            _building_type = BUILD_TYPE.NINE_BLOCK;
        }
        if ((int)t_build["build_type"] == 22)
        {
            _building_type = BUILD_TYPE.FOUR_BLOCK;
        }

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

        MapManager.getInstance().toogleMapsState();


    }

    // maps are not opened
    public bool startBuilding(string id,bool is_enemy)
    {
        Dictionary<string, object> t_build = findMapFromId(id);

        if (t_build == null)
            return false;

        readDictionary(t_build, "min", ref _min);
        readDictionary(t_build, "gas", ref _gas);

        int temp_min = 0;
        int temp_gas = 0;

        if (is_enemy)
        {
            temp_min = _resources2.getMin();
            temp_gas = _resources2.getGas();

        }
        else
        {
            temp_min = _resources1.getMin();
            temp_gas = _resources1.getGas();
        }
        

        if (_min > temp_min)
        {
            MessageManager.getInstance().popupMessage("미네랄이 부족합니다.");
            return false;
        }

        if (_gas > temp_gas)
        {
            MessageManager.getInstance().popupMessage("가스가 부족합니다.");
            return false;
        }

        if ((int)t_build["build_type"] == 33)
        {
            _building_type = BUILD_TYPE.NINE_BLOCK;
        }

        if (id == "2001")
        {
            _target_to_build = Resources.Load("Prefabs/Building/King") as GameObject;
        }

        return true;
    }
    

	public Dictionary<string,object> findMapFromId(string id)
	{
		for (int i = 0; i < data.Count; i++)
		{
			//int iid = (int)data[i]["id"];
			if ((int)data[i]["id"] == int.Parse(id))
			{
				return data [i];
			}
		}

		return null;
	}

    public void commandBuild(float pos_x,float pos_y,float pos_z,string name,bool is_enemy)
    {
  

        if (is_enemy)
        {
            _resources2.setMin(_resources2.getMin() - (int)_min);
            _resources2.setGas(_resources2.getGas() - (int)_gas);
        }
        else
        {
            _resources1.setMin(_resources1.getMin() - (int)_min);
            _resources1.setGas(_resources1.getGas() - (int)_gas);


        }

        

        GameObject _target_to_build = getTargetBuilding();
        BUILD_TYPE building_type = getBuildingType();

        Vector3 position = new Vector3(pos_x , pos_y, pos_z);

		if (building_type == ConstructionManager.BUILD_TYPE.NINE_BLOCK)
        {
			position = new Vector3(pos_x + 1 , pos_y, pos_z + 1);

            string object_name = name;
            string[] map_position = object_name.Split('p');

            int x = int.Parse(map_position[1]);
            int y = int.Parse(map_position[2]);

            if (!isMapBuildable(x + 1, y) || !isMapBuildable(x, y + 1) || !isMapBuildable(x + 1, y + 1) || 
				!isMapBuildable(x + 2, y )||!isMapBuildable(x + 2, y + 1)||!isMapBuildable(x + 2, y + 2)||
				!isMapBuildable(x, y + 2)||!isMapBuildable(x + 1, y + 2))
            {
                MessageManager.getInstance().popupMessage("건설이 불가능 합니다.");
                return;
            }
        }

        if (position != null)
        {
            GameObject building = Instantiate(_target_to_build, position, transform.rotation);
            building.GetComponent<UnitStaus>()._is_enemy = is_enemy;
            building.GetComponent<UnitStaus>()._is_building = true;

            string object_name = name;
            string[] map_position = object_name.Split('p');

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
