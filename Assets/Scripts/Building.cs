using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

    public float _build_time = 1f;

    Color _current_color = new Color(1, 1, 1, 0.2f);
    Color _last_color = new Color(1, 1, 1, 1);

    public Material _material;

    void Start ()
    {
        _material = Resources.Load("Materials/Medieval_Material") as Material;

        gameObject.GetComponent<Renderer>().material.color = _current_color;
        StartCoroutine("init");
    }


    IEnumerator init()
    {

        float _current_time = 0;

        while (true)
        { 
            _current_time += Time.deltaTime;

            //Debug.Log(_current_time);

            if (_current_time < _build_time)
            {
                gameObject.GetComponent<Renderer>().material.color = Color.Lerp(_current_color, _last_color, _current_time / _build_time);
            }
            else
            {
                break;
            }

         


            yield return null;
        }

        Material[] mats = GetComponent<Renderer>().materials;
        mats[0] = _material;
        GetComponent<Renderer>().materials = mats;


        yield return null;
    }




    // Update is called once per frame
    void Update ()
    {
    }
}
