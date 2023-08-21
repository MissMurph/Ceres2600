using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ceres.Player {

	public class GrapplingHook : MonoBehaviour {
		[SerializeField]
		private float grappleAcceleration;

		[SerializeField]
		private GameObject hookPrefab;

		private bool grappling;
		private Transform grapplePoint;
		private LineRenderer grappleLine;
		[SerializeField]
		private float distance;
		private float initialDistance;
		private float initialMoveModifier;

		[SerializeField]
		private float gravityCancelThreshold;

		[SerializeField]
		private float grappleEndThreshold;

		[SerializeField]
		private float grappleProgress;

		private LookRotation rotator;
		private GroundDetection ground;
		private Movement motion;
		private Rigidbody physics;

		private void Awake () {
			rotator = GetComponent<LookRotation>();
			ground = GetComponent<GroundDetection>();
			motion = GetComponent<Movement>();
			physics = GetComponent<Rigidbody>();
		}

		private void Update () {
			if (grappling) grappleLine.SetPositions(new Vector3[] { grapplePoint.position, transform.position + Vector3.left * 0.1f });
		}
		private void FixedUpdate () {
			if (grappling) ApplyGrappleMotion();
		}

		private void ApplyGrappleMotion () {
			Vector3 grappleDirection = grapplePoint.transform.position - transform.position;
			float distanceToAnchor = grappleDirection.magnitude;

			grappleProgress = distanceToAnchor / initialDistance;

			/*if (distance < distanceToAnchor) {
				//float velocity = physics.velocity.magnitude;
				Vector3 newDirection = Vector3.ProjectOnPlane(physics.velocity, grappleDirection);
				//physics.velocity = newDirection.normalized * velocity;
				//physics.AddForce(newDirection.normalized * grappleAcceleration, ForceMode.Acceleration);
			}
			else physics.AddForce(grappleDirection.normalized * grappleAcceleration, ForceMode.Acceleration);*/

			if (grappleProgress <= gravityCancelThreshold) {
				physics.useGravity = false;
			}
			else {
				physics.useGravity = true;
			}

			if (grappleProgress <= grappleEndThreshold) {
				EndGrapple();
				return;
			}

			motion.MoveMultiplier = initialMoveModifier * Mathf.Clamp01(grappleProgress);

			physics.AddForce(grappleDirection.normalized * grappleAcceleration * Mathf.Clamp01(grappleProgress), ForceMode.Acceleration);
		}

		public void Grapple (InputAction.CallbackContext context) {
			if (context.performed) {
				FireGrapple();
			}

			if (context.canceled) {
				EndGrapple();
			}
		}

		private void FireGrapple () {
			Ray ray = new(transform.position, rotator.ViewPort.transform.forward);

			if (Physics.Raycast(ray, out RaycastHit hit, 100f, ground.WorldLayer)) {
				grapplePoint = Instantiate(hookPrefab, hit.point, Quaternion.Euler(Vector3.zero)).transform;
				initialDistance = (grapplePoint.transform.position - transform.position).magnitude;
				initialMoveModifier = motion.MoveMultiplier;
				grappleLine = grapplePoint.GetComponent<LineRenderer>();
				grappling = true;
				return;
			}
		}

		private void EndGrapple () {
			grappling = false;
			physics.useGravity = true;

			if (grapplePoint != null) {
				initialDistance = 0f;
				motion.MoveMultiplier = initialMoveModifier;
				initialMoveModifier = 0f;
				Destroy(grapplePoint.gameObject);
			}
		}
	}
}