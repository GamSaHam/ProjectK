using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFinish : StateMachineBehaviour {
	public float _time = .5f;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	//override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Debug.Log ("hi");
	}
	bool _is_attack = false;
	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//	Debug.Log ("hi1");
		if (stateInfo.normalizedTime < _time) {
			_is_attack = false;
		
		}

		if (stateInfo.normalizedTime > _time) {
			if (_is_attack == false) {
				
				EventManager.getInstance ().executeAttackEvent (animator.GetHashCode());

				_is_attack = true;
			}
		}
	}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//	Debug.Log ("hi2");
	}
}
