using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

	public static EventManager _instance;

	private void Awake()
	{
		if (_instance != null)
		{
			Debug.LogError("More than one EventManager in scene");
		}

		_instance = this;
	}

	public static EventManager getInstance()
	{
		return _instance;
	}

	public delegate void AttackEvent(GameObject target);


	class EventAttack
	{
		public AttackEvent _attack_event;
		public GameObject _origin;
		public GameObject _target;
		public int _hash_code;

		public EventAttack(AttackEvent attack_event, GameObject origin,GameObject target,int hash_code)
		{
			_attack_event = attack_event;
			_origin = origin;
			_target = target;
			_hash_code = hash_code;
		}
	}

	List<EventAttack> _event_list = new List<EventAttack>();

	public void addAttackEvent(AttackEvent attack_event,GameObject origin,GameObject target,int hash_code)
	{
		_event_list.Add (new EventAttack (attack_event, origin,target,hash_code));

	}

	public void executeAttackEvent(int hash_code)
	{
		for (int i = 0; i < _event_list.Count; i++) {
			EventAttack event_attack = _event_list [i];

			if (hash_code == event_attack._hash_code) 
			{
				event_attack._attack_event (event_attack._target);
				break;
			}
		}

		//_attack_event (_obj);

	}

	public void removeEvent(GameObject obj)
	{
		
		for (int i = 0; i < _event_list.Count; i++) {
			EventAttack event_attack = _event_list [i];

			if (event_attack._origin == obj) 
			{
				_event_list.RemoveAt (i);
				break;
			}
		}
	}


	public void doSomething(object target)
	{


	}


}
