using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableSkill : MonoBehaviour {

	public static TableSkill _instance;

	public static TableSkill getInstance()
	{
		return _instance;
	}


	public List<Dictionary<string, object>> data;

	void Awake()
	{
		if (_instance != null)
		{
			Debug.LogError("More than one BuildingManager in scene");
		}

		_instance = this;

			data = CSVReader.Read("CSV/table_skill");

	}



	public float getTime(int id)
	{
		for (int i = 0; i < data.Count; i++)
		{
			Dictionary<string, object> dic = data [i];

			int temp_id = (int)dic["id"];

			if (temp_id == id) 
			{
				float limited_time = 0;
				Table.readDictionary (dic, "limited_time",ref limited_time);

				return limited_time;
			}
		}

		return -1;
	}

	public float getUpRate(int id)
	{
		for (int i = 0; i < data.Count; i++)
		{
			Dictionary<string, object> dic = data [i];

			int temp_id = (int)dic["id"];

			if (temp_id == id) 
			{
				float up_rate = 0;
				Table.readDictionary (dic, "up_rate",ref up_rate);
				return up_rate;

			}
		}

		return -1;
	}

}
