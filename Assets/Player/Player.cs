using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

	[SerializeField]
	private float maxMoveSpeed;

	[SerializeField]
	private float acceleration;

	private float currentSpeed;

	private Vector2 moveDirection;

	//This will be the delta of the mouse, how quickly we need to turn
	//x delta needs to be rotated around the y axis
	//y delta needs to be rotated around the x axis, clamped between -90 & 90
	private Vector2 lookDelta;

	private float verticalAngle;
	private float horizontalAngle;

	[SerializeField]
	private Vector2 sensitivity;

	[SerializeField]
	private float jumpHeight;

	private Camera view;

	private CharacterController controller;

	[SerializeField]
	private float grappleAcceleration;
	[SerializeField]
	private float grappleMaxSpeed;
	private float grappleSpeed;

	private bool grappling;
	private Transform grapplePoint;

	private bool grounded {
		get {
			return controller.isGrounded;
		}
	}

	private float VerticalVelocity {
		get {
			return velocityVector.y;
		}
		set {
			velocityVector.y = value;
		}
	}

	private Vector3 velocityVector = new();

	[SerializeField]
	private float gravity = -9.8f;

	[SerializeField]
	private LayerMask worldLayer;

	private void Awake () {
		view = GetComponentInChildren<Camera>();
		controller = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update () {
		UpdateRotation();
		UpdateMotion();
	}

	private void UpdateRotation () {
		//Reading rotation data from Unity is impractical due to complex Quaternion math
		//In order to maintain consistency with looking, we need to create or own angles and force Unity to match
		//Thus we store the angles as separate consistent variables that we send to Unity
		horizontalAngle += lookDelta.x * sensitivity.x;
		verticalAngle -= lookDelta.y * sensitivity.y;

		//Don't want to be able to look past each pole, so we clamp
		verticalAngle = Mathf.Clamp(verticalAngle, -90f, 90f);

		//TODO: come up with smoother mouse movement, lerp somewhere
		view.transform.localRotation = Quaternion.Euler(verticalAngle, 0, 0);
		transform.rotation = Quaternion.Euler(0, horizontalAngle, 0);
	}

	private void UpdateMotion () {
		//Check if grounded to reset vertical movement
		if (grounded && VerticalVelocity < 0) {
			VerticalVelocity = 0f;
		}

		Vector3 forwardsMotion = transform.forward * moveDirection.y;
		Vector3 horizontalMotion = transform.right * moveDirection.x;

		Vector3 motion = (forwardsMotion + horizontalMotion).normalized;

		if (grappling) {
			motion = ApplyGrappleMotion(motion);
		}

		//Add acceleration amount
		currentSpeed += acceleration * Time.deltaTime;

		//Clamp the new velocity so it doesn't exceed max speed
		currentSpeed = Mathf.Min(maxMoveSpeed, currentSpeed);

		controller.Move(motion * currentSpeed * Time.deltaTime);

		if (!grappling) VerticalVelocity += gravity * Time.deltaTime;

		controller.Move(velocityVector * Time.deltaTime);
	}

	private Vector3 ApplyGrappleMotion (Vector3 motion) {
		Vector3 grappleDirection = (grapplePoint.transform.position - transform.position).normalized;

		grappleSpeed += grappleAcceleration * Time.deltaTime;
		grappleSpeed = Mathf.Min(grappleMaxSpeed, grappleSpeed);

		grappleDirection *= grappleSpeed * Time.deltaTime;

		motion += grappleDirection;

		return motion;
	}

	public void Move (InputAction.CallbackContext context) {
		moveDirection = context.ReadValue<Vector2>();
	}

	public void Look (InputAction.CallbackContext context) {
		lookDelta = context.ReadValue<Vector2>();
	}

	public void Fire (InputAction.CallbackContext context) {
		
	}

	public void AltFire (InputAction.CallbackContext context) {
		
	}

	public void Grapple (InputAction.CallbackContext context) {
		if (context.performed) {
			Ray ray = new Ray(transform.position, transform.forward);

			if (Physics.Raycast(ray, out RaycastHit hit, 100f, worldLayer)) {
				grapplePoint = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), hit.transform.position, Quaternion.Euler(Vector3.zero)).transform;
				grappling = true;
			}
		}

		if (context.canceled) {
			grappling = false;
			Destroy(grapplePoint.gameObject);
		}
	}

	public void Grab (InputAction.CallbackContext context) {

	}

	public void Reload (InputAction.CallbackContext context) {

	}

	public void Jump (InputAction.CallbackContext context) {
		if (context.performed && grounded) {
			VerticalVelocity += jumpHeight;
		}
	}

	public void Crouch (InputAction.CallbackContext context) {

	}
}