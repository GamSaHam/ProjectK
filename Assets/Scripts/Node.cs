﻿using UnityEngine;
using FoW;

public class Node : MonoBehaviour
{
    public Material _material;
    public FogOfWar _fog;
    public bool _is_builable = true;

	public bool _is_open = false;

	MeshRenderer _mesh_renderer;
    public Construction _construction;

    void Awake()
    {
        _fog = GameObject.Find("Main Camera").GetComponent<FogOfWar>();
        _material = GetComponent<MeshRenderer>().materials[0];
        _material.color = Color.green;
		_mesh_renderer = GetComponent<MeshRenderer> ();
        _construction = GameObject.Find("Player1").GetComponent<Construction>();

        if (_is_builable == false)
        {
            _material.color = Color.red;
        }
    }

    public byte[] select_data;

	public void setMapState(bool is_open)
	{
		_is_open = is_open;

		if (_is_open == true) {
			_mesh_renderer.enabled = true;
		
		} else {
		
			_mesh_renderer.enabled = false;
		}

	}

    private void OnMouseDown()
    {
		if (!_is_open)
			return;
			
        // 경계 설정
        if (Input.mousePosition.y < 230)
        {
            return;
        }

        if (_is_builable == false)
        {
            MessageManager.getInstance().popupMessage("건설이 불가능 합니다.");
            return;
        }
			
        if (_fog.IsInCompleteFog(transform.position))
        {
            MessageManager.getInstance().popupMessage("시야를 확보해 주세요.");
            return;
        }



        _construction.commandBuild(transform.position.x, transform.position.y, transform.position.z,gameObject.name,false);

      
       
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
		if (!_is_open) {
			return;
		}

        GameObject _target_to_build = _construction.getTargetBuilding();
        BUILD_TYPE building_type = _construction.getBuildingType();

        if (building_type == BUILD_TYPE.FOUR_BLOCK)
        {
            string object_name = gameObject.name;

            string[] map_position = object_name.Split('p');

            

            changeMapColor(map_position, 0, 1, true);
            changeMapColor(map_position, 1, 0, true);
            changeMapColor(map_position, 1, 1, true);
        }

		if (building_type == BUILD_TYPE.NINE_BLOCK)
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
		if (!_is_open)
			return;

        GameObject _target_to_build = _construction.getTargetBuilding();
        BUILD_TYPE building_type = _construction.getBuildingType();

        if (building_type == BUILD_TYPE.FOUR_BLOCK)
        {
            string object_name = gameObject.name;
            string[] map_position = object_name.Split('p');

            changeMapColor(map_position, 0, 1, false);
            changeMapColor(map_position, 1, 0, false);
            changeMapColor(map_position, 1, 1, false);
        }

		if (building_type == BUILD_TYPE.NINE_BLOCK)
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


