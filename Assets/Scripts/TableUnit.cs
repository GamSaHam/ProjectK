using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableUnit : MonoBehaviour {

	public static TableUnit _instance;

	public static TableUnit getInstance()
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

		//File.WriteAllText(Application.dataPath + "/Resources/CSV/example.csv", File.ReadAllText(Application.dataPath + "/Resources/CSV/example.csv", Encoding.ASCII), Encoding.UTF8);
		data = CSVReader.Read("CSV/table_unit");

		for (int i = 0; i < data.Count; i++)
		{
			print("id " + data[i]["id"] + " " +
				"radius " + data[i]["radius"]);
		}
	}
}
