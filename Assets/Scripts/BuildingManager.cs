﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour {

    public static BuildingManager _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("More than one BuildingManager in scene");
        }

        _instance = this;
    }

    public static BuildingManager getInstance()
    {
        return _instance;
    }

    class BuildingState
    {
        GameObject _building;
        int _x;
        int _y;
        ConstructionManager.BUILD_TYPE _type;
        
        public BuildingState(GameObject building,int x,int y, ConstructionManager.BUILD_TYPE type)
        {
            _building = building;
            _x = x;
            _y = y;
            _type = type;
            
        }

    }

    List<BuildingState> _number_of_buildings = new List<BuildingState>();


    public void insertBuilding(GameObject game_object,int x,int y,ConstructionManager.BUILD_TYPE build_type)
    {

        MapManager.getInstance().unableToBuild(x, y);
        
        if (build_type == ConstructionManager.BUILD_TYPE.FOUR_BLOCK)
        {
            MapManager.getInstance().unableToBuild(x+1, y);
            MapManager.getInstance().unableToBuild(x, y+1);
            MapManager.getInstance().unableToBuild(x+1, y+1);

        }


        BuildingState building_state = new BuildingState(game_object, x, y, build_type);
        _number_of_buildings.Add(building_state);





    }

    // Use this for initialization
    void Start ()
    {
        GameObject king_tower1 = GameObject.Find("KingTower1");
        insertBuilding(king_tower1,5,5,ConstructionManager.BUILD_TYPE.FOUR_BLOCK);




    }

    // Update is called once per frame
    void Update () {
		
	}
}
