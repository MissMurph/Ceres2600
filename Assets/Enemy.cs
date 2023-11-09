using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

	[SerializeField]
	private Transform trackedTarget;
	private NavMeshAgent navigator;

	private void Awake () {
		navigator = GetComponent<NavMeshAgent>();
	}

	private void Update () {
		navigator.destination = trackedTarget.position;
	}
}