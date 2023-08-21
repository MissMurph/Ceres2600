using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Ceres.Player {

	public class PhysicsControl : MonoBehaviour {

		[SerializeField]
		private float groundDrag;
		[SerializeField]
		private float airDrag;
		[SerializeField]
		private float slideDrag;
		[SerializeField]
		private float slopeFriction;

		private Player master;

		private GroundDetection ground;
		private CrouchController crouch;
		private Rigidbody physics;

		private void Awake () {
			ground = GetComponent<GroundDetection>();
			crouch = GetComponent<CrouchController>();
			physics = GetComponent<Rigidbody>();
			physics.freezeRotation = true;
		}

		private void Update () {
			ControlDrag();
			ControlFriction();
		}

		private void ControlDrag () {
			if (ground.Grounded) {
				if (crouch.Sliding) {
					physics.drag = slideDrag;
				}
				else physics.drag = groundDrag;
			}
			else physics.drag = airDrag;
		}

		private void ControlFriction () {
			if (ground.Grounded && ground.OnSlope && !crouch.Sliding) {
				ground.Bounds.material.staticFriction = slopeFriction;
			}
			else {
				ground.Bounds.material.staticFriction = 0f;
			}
		}
	}
}