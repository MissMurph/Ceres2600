using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Ceres.Player {

	public class CrouchController : MonoBehaviour {

		//How much time it takes to move between crouched and not crouched states
		[SerializeField]
		private float crouchSpeed;
		[SerializeField]
		private Transform groundCheck;
		[SerializeField]
		private Transform cameraPos;
		[SerializeField]
		private float headRadius;
		[SerializeField]
		private float headOffset;
		[SerializeField]
		private LayerMask worldLayer;
		[SerializeField]
		private float height;

		private bool crouching;
		private bool sliding;
		private bool headCheck;

		private GroundDetection ground;
		private Movement movement;

		public bool Crouching {
			get {
				return crouching;
			}
		}

		public bool Sliding {
			get {
				return sliding;
			}
		}

		private void Awake () {
			ground = GetComponent<GroundDetection>();
			movement = GetComponent<Movement>();
		}

		private void Update () {
			headCheck = Physics.CheckSphere(cameraPos.position + Vector3.one * headOffset, headRadius, worldLayer);

			if (movement.Speed < 5f) sliding = false;
			if (crouching) {
				float change = crouchSpeed * Time.deltaTime;
				Vector3 currentCenter = ground.Bounds.center;

				currentCenter.y += change;
				currentCenter.y = Mathf.Min(currentCenter.y, 0.5f);

				ground.Bounds.height = Mathf.Max(ground.Bounds.height - change, height / 2f);
				//ground.Bounds.center = currentCenter;
			}
			else if (!headCheck) {
				float change = crouchSpeed * Time.deltaTime;
				Vector3 currentCenter = ground.Bounds.center;

				currentCenter.y -= change;
				currentCenter.y = Mathf.Max(currentCenter.y, 0f);

				ground.Bounds.height = Mathf.Min(ground.Bounds.height + change, height);
				//ground.Bounds.center = currentCenter;
			}

			cameraPos.localPosition = new Vector3(0, ground.Bounds.center.y - headOffset + (ground.Bounds.height / 2f), 0);
			groundCheck.localPosition = new Vector3(0, ground.Bounds.center.y - ground.Bounds.height / 2f, 0);
		}

		public void Crouch (InputAction.CallbackContext context) {
			if (context.performed) {
				if (ground.Grounded && (ground.OnSlope || movement.Speed >= 15f)) sliding = true;
				crouching = true;
			}

			if (context.canceled) {
				crouching = false;
				sliding = false;
			}
		}
	}
}