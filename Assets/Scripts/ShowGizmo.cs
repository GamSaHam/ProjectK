using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGizmo : MonoBehaviour {


    public BUILD_TYPE byild_type;




    void OnDrawGizmosSelected()
    {
        float radius = BuildingManager.getRadius(byild_type);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

    }

  



}
