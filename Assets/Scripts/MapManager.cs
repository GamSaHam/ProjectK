using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public static MapManager _instance;

    public GameObject _building_object;
    public GameObject _target_to_build;

    public GameObject[,] _maps = new GameObject[12,12];
    // Use this for initialization
    void Awake ()
    {
        if (_instance != null)
        {
            Debug.LogError("More than one MapManager in scene");
        }

        _instance = this;

        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                if ((i == 11 && j == 11) || (i == 0 && j == 11))
                {
                    continue;
                }

                GameObject make_object = Instantiate(_target_to_build, new Vector3(-31.5f + (j*10) , 0.5f, -45.5f +(i*10)), Quaternion.identity);
                make_object.transform.parent = _building_object.transform;

                string name = string.Format("SF_Env_Tile_Grass_p{0}p{1}",i,j);
                make_object.name = name;

                _maps[i,j] = make_object;
            }
        }

    }

    public static MapManager getInstance()
    {
        return _instance;
    }

    void Start()
    {
        
        unableToBuild(2, 2);
        unableToBuild(2, 5);
        unableToBuild(3, 4);
        
    }

    public void unableToBuild(int x,int y)
    {


        GameObject map = _maps[x, y];

        if (map)
        {
         
            map.GetComponent<Node>().setBuildable(false);

        }
    }

}
