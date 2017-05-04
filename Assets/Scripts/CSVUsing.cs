using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CSVUsing : MonoBehaviour {

    void Awake()
    {
        //File.WriteAllText(Application.dataPath + "/Resources/CSV/example.csv", File.ReadAllText(Application.dataPath + "/Resources/CSV/example.csv", Encoding.ASCII), Encoding.UTF8);
        List<Dictionary<string, object>> data = CSVReader.Read("CSV/table_building");

        for (int i = 0; i < data.Count; i++)
        {
            print("id " + data[i]["id"] + " " +
                   "min " + data[i]["min"] + " " +
                   "gas " + data[i]["gas"] + " " +
                   "description " + data[i]["description"]);
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
