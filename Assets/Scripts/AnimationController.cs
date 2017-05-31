using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {
	public Animator _animator;
	public ANIMATION_STATE _animation_state;
	public Animator _animator_animal;


	public enum ANIMATION_STATE
	{
		IDLE = 1,
		RUN = 2,
		ATTACK = 3


	}

	public void SetState(ANIMATION_STATE state)
	{
		_animation_state = state;


	}


	void Update()
	{
		if (_animation_state == ANIMATION_STATE.IDLE) {
			//set that character is moving
			if (_animator) 
			{
				_animator.SetBool ("Moving", false);
				_animator.SetBool ("Running", false);
			}

			if (_animator_animal != null) {
				_animator_animal.SetBool ("Run", false);
			}
	
		}

		if (_animation_state == ANIMATION_STATE.RUN) {
			if (_animator) {
				_animator.SetBool ("Moving", true);
				_animator.SetBool ("Running", true);
			}
			if (_animator_animal != null) {
				_animator_animal.SetBool ("Run", true);
		
			}
			
		}

		if (_animation_state == ANIMATION_STATE.ATTACK) {
		
			//if(animator.GetBool("Moving") == false)
			//	animator.SetTrigger("Attack1Trigger");
			if (_animator) {
				_animator.SetBool ("Moving", false);
				_animator.SetBool ("Running", false);
			}
			if (_animator_animal != null) {
				_animator_animal.SetBool ("Run", false);
			}

		}


	
	}

	public void triggerAttack()
	{
		
		_animator.SetTrigger ("Attack1Trigger");
		
	}


}
