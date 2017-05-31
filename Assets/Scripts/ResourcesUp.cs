using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesUp : MonoBehaviour {


    public CResources _resources;
	// Use this for initialization
	void Start ()
    {
        if (GetComponent<UnitStaus>()._is_enemy)
        {
            _resources = GameObject.Find("Player2").GetComponent<CResources>();
        }
        else
        {
            _resources = GameObject.Find("Player1").GetComponent<CResources>();
        }

        StartCoroutine("minUp");
	}

    IEnumerator minUp()
    {

        while (true)
        {
            yield return new WaitForSeconds(6f);
            _resources._min += 1;

            
        }

    }

}
