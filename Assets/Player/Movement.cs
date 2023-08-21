using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ceres.Player {

	public class Movement : MonoBehaviour {

		[SerializeField]
		private float moveSpeed;

		[SerializeField]
		private float movementMultiplier;

		public float MoveMultiplier {
			get {
				return movementMultiplier;
			}
			set {
				movementMultiplier = value;
			}
		}

		[SerializeField]
		private float crouchMultiplier;

		[SerializeField]
		private float airMultiplier;

		[SerializeField]
		private float jumpForce;

		[SerializeField]
		private Vector3 direction;

		private Vector2 moveInput;

		[SerializeField]
		private Vector3 motion;

		[SerializeField]
		private float speed;

		[SerializeField]
		private float slideForce;

		[SerializeField]
		private Animator legsAnim;

		private float speedUp;

		private Vector3 slopeMoveDirection;

		private Vector3 forwardsMotion;
		private Vector3 horizontalMotion;

		public Vector3 Direction {
			get {
				return direction;
			}
		}

		public Vector3 Motion {
			get {
				return motion;
			}
		}

		public float Speed {
			get {
				return speed;
			}
		}

		private Rigidbody physics;
		private CrouchController crouch;
		private LookRotation mouseLook;
		private GroundDetection ground;

		private void Awake () {
			physics = GetComponent<Rigidbody>();
			crouch = GetComponent<CrouchController>();
			mouseLook = GetComponent<LookRotation>();
			ground = GetComponent<GroundDetection>();
		}

		private void Update () {
			forwardsMotion = crouch.Sliding ? Vector3.zero : mouseLook.Rotation.forward * moveInput.y;
			horizontalMotion = crouch.Sliding ? Vector3.zero : mouseLook.Rotation.right * moveInput.x;
		}

		private void FixedUpdate () {
			slopeMoveDirection = Vector3.ProjectOnPlane(forwardsMotion + horizontalMotion, ground.Slope.normal);

			if (crouch.Sliding) slopeMoveDirection = Vector3.ProjectOnPlane(mouseLook.Rotation.forward, ground.Slope.normal);

			Vector3 motionDirection = (forwardsMotion + horizontalMotion).normalized;
			float motionSpeed = moveSpeed * movementMultiplier;

			if (ground.Grounded) {
				if (crouch.Crouching) {
					motionSpeed *= crouchMultiplier;
				}

				if (ground.OnSlope) {
					motionDirection = slopeMoveDirection.normalized;
				}
			}

			if (!ground.Grounded) {
				motionSpeed *= airMultiplier;
			}

			direction = motionDirection;

			motion = direction * motionSpeed;

			physics.AddForce(motion, ForceMode.Acceleration);

			speed = physics.velocity.magnitude;

			if (forwardsMotion != Vector3.zero || horizontalMotion != Vector3.zero) speedUp = Mathf.Clamp01(speedUp + Time.fixedDeltaTime);
			else speedUp = Mathf.Clamp01(speedUp - Time.fixedDeltaTime);

			legsAnim.SetFloat("speedPercent", speedUp);
		}

		public void Jump (InputAction.CallbackContext context) {
			if (context.performed && ground.Grounded) {
				physics.velocity = new Vector3(physics.velocity.x, 0, physics.velocity.z);
				physics.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
			}
		}

		public void Move (InputAction.CallbackContext context) {
			moveInput = context.ReadValue<Vector2>();
		}

		public void Crouch (InputAction.CallbackContext context) {
			if (context.performed && ground.Grounded && (ground.OnSlope || speed >= 15f)) {
				physics.AddForce(physics.velocity.normalized * slideForce, ForceMode.Impulse);
			}
		}
	}
}