using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CResources : MonoBehaviour
{

    public int _min = 200;
    //public int _min = 2000;
    public int _gas = 0;

    float _time = 0;

    public Text _text_min;
    public Text _text_gas;
    
	// Update is called once per frame
	void FixedUpdate ()
    {
        _time += Time.deltaTime;

        if (_time > 1.0f)
        {
            _time -= 1;

            _min += 1;
            //gas += 1;

            invalidate();
        }
    }

    public void invalidate()
    {
        if (_text_min)
        {
            _text_min.text = _min.ToString();
            _text_gas.text = _gas.ToString();
        }
    }

    public int getMin()
    {
        return _min;
    }

    public void setMin(int min)
    {
        _min = min;
        invalidate();
    }

    public int getGas()
    {
        return _gas;
    }

    public void setGas(int gas)
    {
        _gas = gas;
        invalidate();
    }



}
