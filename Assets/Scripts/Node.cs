using UnityEngine;
using FoW;

public class Node : MonoBehaviour
{
    public Material _material;
    public FogOfWar _fog;
    public bool _is_builable = true;

    void Awake()
    {
        _fog = GameObject.Find("Main Camera").GetComponent<FogOfWar>();
        _material = GetComponent<MeshRenderer>().materials[0];
        _material.color = Color.green;

        if (_is_builable == false)
        {
            _material.color = Color.red;
        }
    }

    public byte[] select_data;

    private void OnMouseDown()
    {
        if (Input.mousePosition.y < 230)
        {
            return;
        }
        if (_is_builable == false)
        {
            MessageManager.getInstance().popupMessage("건설이 불가능 합니다.");

            return;

        }

        byte[] original = _fog.texture.GetRawTextureData();

        if (_fog.IsInCompleteFog(transform.position))
        {
            MessageManager.getInstance().popupMessage("시야를 확보해 주세요.");
            return;
        }

        ConstructionManager._instance.commandBuild(transform.position.x, transform.position.y, transform.position.z,gameObject.name);

      
       
    }

    void changeMapColor(string[] map_position, int x2,int y2,bool is_red)
    {
        int x = int.Parse(map_position[1]);
        int y = int.Parse(map_position[2]);

        x = x + x2;
        y = y + y2;

        string tile_name = string.Format("SF_Env_Tile_Grass_p{0}p{1}", x, y);
        GameObject map = GameObject.Find(tile_name);
        
        if (map)
        {
            if (map.GetComponent<Node>()._is_builable == false)
            {
                
                return;
            }

            if(is_red)
                map.GetComponent<Node>()._material.color = Color.red;
            else
                map.GetComponent<Node>()._material.color = Color.green;
        }
    }



    private void OnMouseEnter()
    {
        GameObject _target_to_build = ConstructionManager._instance.getTargetBuilding();
        ConstructionManager.BUILD_TYPE building_type = ConstructionManager._instance.getBuildingType();

        if (building_type == ConstructionManager.BUILD_TYPE.FOUR_BLOCK)
        {
            string object_name = gameObject.name;

            string[] map_position = object_name.Split('p');

            

            changeMapColor(map_position, 0, 1, true);
            changeMapColor(map_position, 1, 0, true);
            changeMapColor(map_position, 1, 1, true);
        }

		if (building_type == ConstructionManager.BUILD_TYPE.NINE_BLOCK)
		{
			string object_name = gameObject.name;

			string[] map_position = object_name.Split('p');


			changeMapColor(map_position, 0, 1, true);
			changeMapColor(map_position, 1, 0, true);
			changeMapColor(map_position, 1, 1, true);

			changeMapColor(map_position, 2, 0, true);
			changeMapColor(map_position, 2, 1, true);
			changeMapColor(map_position, 2, 2, true);
			changeMapColor(map_position, 0, 2, true);
			changeMapColor(map_position, 1, 2, true);
		}

        if(_is_builable)
            _material.color = Color.red;
    }



    private void OnMouseExit()
    {
        GameObject _target_to_build = ConstructionManager._instance.getTargetBuilding();
        ConstructionManager.BUILD_TYPE building_type = ConstructionManager._instance.getBuildingType();

        if (building_type == ConstructionManager.BUILD_TYPE.FOUR_BLOCK)
        {
            string object_name = gameObject.name;
            string[] map_position = object_name.Split('p');

            changeMapColor(map_position, 0, 1, false);
            changeMapColor(map_position, 1, 0, false);
            changeMapColor(map_position, 1, 1, false);
        }

		if (building_type == ConstructionManager.BUILD_TYPE.NINE_BLOCK)
		{
			string object_name = gameObject.name;
			string[] map_position = object_name.Split('p');

			changeMapColor(map_position, 0, 1, false);
			changeMapColor(map_position, 1, 0, false);
			changeMapColor(map_position, 1, 1, false);

			changeMapColor(map_position, 2, 0, false);
			changeMapColor(map_position, 2, 1, false);
			changeMapColor(map_position, 2, 2, false);
			changeMapColor(map_position, 0, 2, false);
			changeMapColor(map_position, 1, 2, false);
		}



        if (_is_builable)
            _material.color = Color.green;
    }

    public void setBuildable(bool is_build)
    {
        if (is_build)
            _material.color = Color.green;
        else
            _material.color = Color.red;

        _is_builable = is_build;
    }


}


