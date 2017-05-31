using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirSearch : MonoBehaviour {

    public Vector3 target;

    int targetIndex;

    

    private void Start()
    {
        targetIndex = 0;
    }

    public void StartSearch(Vector3 _target)
    {
        target = _target;
        is_search = true;

    }

    public void StopSearch()
    {
        is_search = false;
        
    }

    public bool is_search = false;

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (is_search == false)
            return;



        Vector3 pos = Vector3.MoveTowards(transform.position, target, GetComponent<UnitStaus>()._current_speed * Time.deltaTime);

        pos.y = transform.position.y;


        Vector3 dir = target - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(dir);

        Vector3 rotation = lookRotation.eulerAngles;





        transform.rotation = Quaternion.Euler(0f,rotation.y,0f);
        transform.position = pos;
           

        
    }
}
