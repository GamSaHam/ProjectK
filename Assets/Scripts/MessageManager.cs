using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour {

    public static MessageManager _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("More than one buildManager in scene");
        }

        _instance = this;
    }

    public static MessageManager getInstance()
    {
        return _instance;
    }

    public Text _text;

    string _message = "";
    bool _is_trigger = false;

    public void popupMessage(string message)
    {
        //   StartCoroutine("animationMessage", (object)message);
        _time = 0;
        _message = message;
        _is_trigger = true;
    }

    float _time = 0;
    
    private void Update()
    {
        if (_is_trigger)
        {
            _time += Time.deltaTime;
            _text.text = _message;
        }

        if (_time > 2.0f)
        {
            _text.text = "";
            _is_trigger = false;
            _time = 0;
        }

       
    }






}
