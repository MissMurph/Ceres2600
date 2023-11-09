using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour {

	private Animator pose;
	//private NavMeshAgent navigator;


	private void Awake () {
		pose = GetComponentInChildren<Animator>();
		//navigator = GetComponent<NavMeshAgent>();
	}


}