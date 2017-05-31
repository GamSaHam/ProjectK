using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

	public const int map_width_size = 20;
	public const int map_hight_size = 20;

    public static MapManager _instance;

    public GameObject _building_object;
    public GameObject _target_to_build;

	public GameObject[,] _maps = new GameObject[map_width_size,map_hight_size];

    void Awake ()
    {
        if (_instance != null)
        {
            Debug.LogError("More than one MapManager in scene");
        }

        _instance = this;

		for (int i = 0; i < map_width_size; i++)
        {
			for (int j = 0; j < map_hight_size; j++)
            {


				GameObject make_object = Instantiate(_target_to_build, Vector3.zero, Quaternion.identity);
                make_object.transform.parent = _building_object.transform;
				make_object.transform.localPosition = new Vector3 (-48.00f + (j * 5), 0.2f, -48.00f + (i * 5));


                string name = string.Format("SF_Env_Tile_Grass_p{0}p{1}",i,j);
                make_object.name = name;
				make_object.GetComponent<Node> ().setMapState (false);

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
      //  unableToBuild(2, 2);
      //  unableToBuild(2, 5);
      //  unableToBuild(3, 4);
        
    }

    public void setBuildable(int x,int y,bool is_able)
    {
        
        GameObject map = _maps[x, y];

        if (map)
        {
            if(is_able)
                map.GetComponent<Node>().setBuildable(true);
            else
                map.GetComponent<Node>().setBuildable(false);
        }
    }

    public bool _is_open = false;
	public void toogleMapsState()
	{
        _is_open = !_is_open;

        for (int i = 0; i < map_width_size; i++) {
			for (int j = 0; j < map_hight_size; j++) {
				_maps [i, j].GetComponent<Node> ().setMapState (_is_open);
			}
		}

	}

}
