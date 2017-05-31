using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerAI : MonoBehaviour 
{
    public Construction _constructuon1;
    public Construction _constructuon2;


    public void makeBuilding(int x,int y,string id,bool is_enemy)
    {
        GameObject _target = MapManager.getInstance()._maps[x, y];

        bool is_build = false;
        if (is_enemy)
            is_build = _constructuon2.startBuilding(id,is_enemy);
        else
            is_build = _constructuon1.startBuilding(id, is_enemy);


        if (is_enemy)
            _constructuon2.commandBuild(_target.transform.position.x, _target.transform.position.y, _target.transform.position.z, _target.gameObject.name, is_enemy);
        else
            _constructuon1.commandBuild(_target.transform.position.x, _target.transform.position.y, _target.transform.position.z, _target.gameObject.name, is_enemy);




    }

    bool _is_action1 = false;
    // 농장을 짓는다.
    void Action1()
    {
        makeBuilding(11, 18, "2002", true);

    }

    int i = 0;
    // 병영을 짓는다
    void Action2()
    {
        makeBuilding(7, 18-(i*2), "2003", true);
        
        if(i<2)
            i++;
    }

    private void Awake()
    {
        _constructuon1 = GameObject.Find("Player1").GetComponent<Construction>();
        _constructuon2 = GameObject.Find("Player2").GetComponent<Construction>();


    }


    void Start () 
	{
        makeBuilding(9,2,"2001",false);
        makeBuilding(9, 18, "2001", true);
    }

    public CResources _resources;

    float time = 0;

    private void Update()
    {
        time += Time.deltaTime;

        if (_resources == null)
            return;


        if (time >= 15)
        {
            if (_is_action1 == false)
            {
                Action1();
                _is_action1 = true;
                time = 0;
            }
        }

        if (time >= 30)
        {
            time = 0;
            
            Action2();
            
            
        }

    }


}
