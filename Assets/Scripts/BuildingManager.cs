using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class BuildingManager : MonoBehaviour 
{
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

    public class BuildingState
    {
        public GameObject _building;
		public int X{ get; set;}
		public int Y{ get; set;}
        public BUILD_TYPE _type;



        public BuildingState(GameObject building,int x,int y, BUILD_TYPE type)
        {
            _building = building;
            X = x;
            Y = y;
            _type = type;
   
        

        }



    }

	public List<BuildingState> _building_list = new List<BuildingState>();


	public void setMapBuildable(int x,int y,bool is_able = false)
	{
		MapManager.getInstance().setBuildable(x, y, is_able);
 
    }

    static public float getRadius(BUILD_TYPE build_type)
    {

        if (build_type == BUILD_TYPE.FOUR_BLOCK)
        {
            return 4.5f;
        }

        return 4.5f;

    }


    public void insertBuilding(GameObject game_object,int x,int y,BUILD_TYPE build_type)
    {
        setMapBuildable(x,y);

        if (build_type == BUILD_TYPE.FOUR_BLOCK)
        {

            setMapBuildable(x+1,y);
            setMapBuildable(x,y+1);
            setMapBuildable(x+1,y+1);

        }

		if (build_type == BUILD_TYPE.NINE_BLOCK)
		{
            setMapBuildable(x+1,y);
            setMapBuildable(x,y+1);
            setMapBuildable(x+1,y+1);

            setMapBuildable(x+2,y);
            setMapBuildable(x+2,y+1);
            setMapBuildable(x+2,y+2);
            setMapBuildable(x,y+2);
            setMapBuildable(x+1,y+2);

		}

        BuildingState building_state = new BuildingState(game_object, x, y, build_type);
        _building_list.Add(building_state);

    }

	public bool isEnterErea(Point p1,BuildingState build_state)
	{
		int start_x = build_state.Y *2 ;
		int start_y = build_state.X *2 ;

		int end_x = 0;
		int end_y = 0;

		if (build_state._type == BUILD_TYPE.NINE_BLOCK) {
			end_x = start_x + 5;
			end_y = start_y + 5;
		
		}

		if (build_state._type == BUILD_TYPE.FOUR_BLOCK) {
		
		
		}

		if (p1.X >= start_x && p1.X <= end_x && p1.Y >= start_y && p1.Y <= end_y) 
		{
			return true;
		}

		return false;
	}

	public BuildingState getBuildingStateByPoint(Point p)
	{
		for (int i = 0; i < _building_list.Count; i++) 
		{
			BuildingState build_state = _building_list[i];
			if (isEnterErea (p, build_state)) {
				return build_state;
			}
		}

		return null;
	}

	public void addOpenList(ref List<Point> open_list,int y,int x)
	{
		Point p1 = new Point (x*2, y*2);
		Point p2 = new Point (x*2 + 1, y*2);
		Point p3 = new Point (x*2, y*2 + 1);
		Point p4 = new Point (x*2 + 1, y*2 + 1);
	
		open_list.Add (p1);
		open_list.Add (p2);
		open_list.Add (p3);
		open_list.Add (p4);
	}

	public List<Point> getPointList(Point p)
	{
		List<Point> open_list = new List<Point> ();

		BuildingState building = getBuildingStateByPoint (p);

		if (building._type == BUILD_TYPE.NINE_BLOCK) 
		{
			for (int i = 0; i < 3; i++) 
			{
				for (int j = 0; j < 3; j++) 
				{
					addOpenList (ref open_list, building.X + i, building.Y + j);
				
				}
			}

		
		}

		return open_list;

	}

    public void removeBuilding(GameObject obj)
    {
        for (int i = 0; i < _building_list.Count; i++)
        {
            if (_building_list[i]._building == obj)
            {
                int x = _building_list[i].X;
                int y = _building_list[i].Y;
                BUILD_TYPE build_type = _building_list[i]._type;
                setMapBuildable(x, y,true);

                if (build_type == BUILD_TYPE.FOUR_BLOCK)
                {

                    setMapBuildable(x + 1, y, true);
                    setMapBuildable(x, y + 1, true);
                    setMapBuildable(x + 1, y + 1, true);

                }

                if (build_type == BUILD_TYPE.NINE_BLOCK)
                {
                    setMapBuildable(x + 1, y, true);
                    setMapBuildable(x, y + 1, true);
                    setMapBuildable(x + 1, y + 1, true);

                    setMapBuildable(x + 2, y, true);
                    setMapBuildable(x + 2, y + 1, true);
                    setMapBuildable(x + 2, y + 2, true);
                    setMapBuildable(x, y + 2, true);
                    setMapBuildable(x + 1, y + 2, true);

                }

                _building_list.RemoveAt(i);

                break;
            }

        }

    }




}
