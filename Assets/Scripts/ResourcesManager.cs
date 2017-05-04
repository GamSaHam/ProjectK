using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("More than one ResourcesManager in scene");
        }

        _instance = this;
    }

    public static ResourcesManager getInstance()
    {
        return _instance;
    }

    private int _min = 1000;
    private int _gas = 1000;

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

        _text_min.text = _min.ToString();
        _text_gas.text = _gas.ToString();

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
