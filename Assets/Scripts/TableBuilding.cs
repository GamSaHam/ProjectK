using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table
{
    static public void readDictionary(Dictionary<string, object> dic, string key, ref float ref_value)
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
}

public class TableBuilding : MonoBehaviour {

    public static TableBuilding _instance;

    public static TableBuilding getInstance()
    {
        return _instance;
    }


    public List<Dictionary<string, object>> data;

    void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("More than one TableBuilding in scene");
        }

        _instance = this;

       
        data = CSVReader.Read("CSV/table_building");
    }


    public Dictionary<string, object> getDictionaryById(int id)
    {
        for (int i = 0; i < data.Count; i++)
        {
            Dictionary<string, object> dic = data[i];

            int temp_id = (int)dic["id"];

            if (temp_id == id)
            {
                return dic;
            }
        }

        return null;

    }

}
