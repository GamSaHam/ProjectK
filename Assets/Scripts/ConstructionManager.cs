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

    public GameObject maps;

    public bool _is_map_open = false;

    public int _min;
    public int _gas;

    public void StartBuilding(string id)
    {
        for (int i = 0; i < data.Count; i++)
        {
            //int iid = (int)data[i]["id"];
            if ((int)data[i]["id"] == int.Parse(id))
            {
                if ((int)data[i]["min"] > ResourcesManager.getInstance().getMin())
                {
                    MessageManager.getInstance().popupMessage("미네랄이 부족합니다.");
                    return;
                }

                if ((int)data[i]["gas"] > ResourcesManager.getInstance().getGas())
                {
                    MessageManager.getInstance().popupMessage("가스가 부족합니다.");
                    return;
                }

                print("id " + data[i]["id"] + " " +
                   "min " + data[i]["min"] + " " +
                   "gas " + data[i]["gas"] + " " +
                   "description " + data[i]["description"] + " " +
                   "build_type " + data[i]["build_type"]);

                _min = (int)data[i]["min"];
                _gas = (int)data[i]["gas"];

                if ((int)data[i]["build_type"] == 33)
                {
					_building_type = BUILD_TYPE.NINE_BLOCK;
                }

                if ((int)data[i]["id"] == 1001)
                {
                    _target_to_build = Resources.Load("Prefabs/FarmBuild") as GameObject;
                }
            }
        }

        if (_is_map_open == false)
        {
            maps.SetActive(true);
            _is_map_open = true;
        }else if (_is_map_open == true)
        {
            maps.SetActive(false);
            _is_map_open = false;
        }
    }

    public void commandBuild(float pos_x,float pos_y,float pos_z,string name)
    {
        ResourcesManager.getInstance().setMin(ResourcesManager.getInstance().getMin() - _min);
        ResourcesManager.getInstance().setGas(ResourcesManager.getInstance().getGas() - _gas);

        GameObject _target_to_build = getTargetBuilding();
        BUILD_TYPE building_type = getBuildingType();

        Vector3 position = new Vector3(pos_x , pos_y, pos_z);

		if (building_type == ConstructionManager.BUILD_TYPE.NINE_BLOCK)
        {
			position = new Vector3(pos_x , pos_y, pos_z);

            string object_name = name;
            string[] map_position = object_name.Split('p');

            int x = int.Parse(map_position[1]);
            int y = int.Parse(map_position[2]);

            if (!isMapBuildable(x + 1, y) || !isMapBuildable(x, y + 1) || !isMapBuildable(x + 1, y + 1) || 
				!isMapBuildable(x + 2, y )||!isMapBuildable(x + 2, y + 1)||!isMapBuildable(x + 2, y + 2)||
				!isMapBuildable(x, y + 2)||!isMapBuildable(x + 1, y + 2))
            {
                return;
            }
        }

        if (position != null)
        {
            GameObject building = Instantiate(_target_to_build, position, transform.rotation);

            string object_name = name;
            string[] map_position = object_name.Split('p');

            int x = int.Parse(map_position[1]);
            int y = int.Parse(map_position[2]);

            BuildingManager.getInstance().insertBuilding(building, x, y, building_type);
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
