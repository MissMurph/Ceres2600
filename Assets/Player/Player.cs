using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

	private static Player instance;

	[Header("Movement")]
	[SerializeField]
	private float speed;

	[SerializeField] 
	private float movementMultiplier;

	[SerializeField]
	private float airMultiplier;

	[SerializeField]
	private float jumpForce;

	[SerializeField]
	private Vector3 direction;

	public Vector3 Direction {
		get {
			return direction;
		}
	}

	[SerializeField]
	private Vector3 motion;

	public static Vector3 Motion {
		get {
			return instance.motion;
		}
	}

	public static Rigidbody Body {
		get {
			return instance.body;
		}
	}

	private Rigidbody body;

	private Vector2 moveInput;
	private Vector3 slopeMoveDirection;

	[Header("Camera")]
	[SerializeField]
	private Camera view;

	public static Camera ViewPort {
		get {
			return instance.view;
		}
	}

	[SerializeField]
	private Vector2 sensitivity;

	[SerializeField]
	private Transform cameraPos;

	[SerializeField]
	private Transform orientation;

	//This will be the delta of the mouse, how quickly we need to turn
	//x delta needs to be rotated around the y axis
	//y delta needs to be rotated around the x axis, clamped between -90 & 90
	private Vector2 lookDelta;

	private float verticalAngle;
	private float horizontalAngle;

	[Header("Ground Detection")]
	[SerializeField]
	private LayerMask worldLayer;

	[SerializeField]
	private float groundDistance;

	[SerializeField]
	private float height;

	[SerializeField]
	private CapsuleCollider bounds;

	[SerializeField]
	private float slopeFriction;

	[SerializeField]
	private Transform groundCheck;

	private RaycastHit slopeHit;

	private bool OnSlope {
		get {
			if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, height / 2 + 0.5f)) {
				if (slopeHit.normal != Vector3.up) return true;
				else return false;
			}
			return false;
		}
	}

	private bool grounded {
		get {
			return Physics.CheckSphere(groundCheck.position, groundDistance, worldLayer);
		}
	}

	[Header("Drag")]
	[SerializeField]
	private float groundDrag;
	[SerializeField]
	private float airDrag;

	[Header("Grappling Hook")]
	[SerializeField]
	private float grappleAcceleration;

	[SerializeField]
	private GameObject hookPrefab;

	private bool grappling;
	private Transform grapplePoint;
	private float distance;

	private void Awake () {
		instance = this;
		body = GetComponent<Rigidbody>();
		body.freezeRotation = true;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update () {
		ControlDrag();
	}

	private void FixedUpdate () {
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
		view.transform.position = cameraPos.position;
		orientation.rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
		view.transform.localRotation = orientation.rotation;
		
	}

	private void UpdateMotion () {
		Vector3 forwardsMotion = orientation.forward * moveInput.y;
		Vector3 horizontalMotion = orientation.right * moveInput.x;

		slopeMoveDirection = Vector3.ProjectOnPlane(forwardsMotion + horizontalMotion, slopeHit.normal);

		Vector3 motionDirection = (forwardsMotion + horizontalMotion).normalized;
		float motionSpeed = speed * movementMultiplier;

		if (grounded && OnSlope) {
			motionDirection = slopeMoveDirection.normalized;
			bounds.material.staticFriction = slopeFriction;
		}
		else {
			bounds.material.staticFriction = 0f;
		}

		if (!grounded) {
			motionSpeed *= airMultiplier;
		}

		direction = motionDirection;

		motion = direction * motionSpeed;

		if (grappling) {
			motion = ApplyGrappleMotion(motion);
		}

		body.AddForce(motion, ForceMode.Acceleration);
	}

	private Vector3 ApplyGrappleMotion (Vector3 motion) {
		Vector3 grappleDirection = grapplePoint.transform.position - transform.position;
		float distanceToAnchor = grappleDirection.magnitude;

		if (distance < distanceToAnchor) {
			float velocity = body.velocity.magnitude;
			Vector3 newDirection = Vector3.ProjectOnPlane(body.velocity, grappleDirection);
			body.velocity = newDirection.normalized * velocity;
		}
		else body.AddForce(grappleDirection.normalized * grappleAcceleration, ForceMode.Acceleration);

		//grappleDirection = grappleDirection.normalized;

		motion += grappleDirection.normalized * grappleAcceleration;

		return motion;
	}

	private void ControlDrag () {
		if (grounded) {
			body.drag = groundDrag;
		}
		else body.drag = airDrag;
	}

	public void Move (InputAction.CallbackContext context) {
		moveInput = context.ReadValue<Vector2>();
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
			Ray ray = new(transform.position, orientation.forward);

			if (Physics.Raycast(ray, out RaycastHit hit, 100f, worldLayer)) {
				grapplePoint = Instantiate(hookPrefab, hit.point, Quaternion.Euler(Vector3.zero)).transform;
				grappling = true;
				return;
			}
		}

		if (context.canceled) {
			grappling = false;
			if (grapplePoint.gameObject != null) Destroy(grapplePoint.gameObject);
		}
	}

	public void Grab (InputAction.CallbackContext context) {

	}

	public void Reload (InputAction.CallbackContext context) {

	}

	public void Jump (InputAction.CallbackContext context) {
		if (context.performed && grounded) {
			body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
			body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}
	}

	public void Crouch (InputAction.CallbackContext context) {

	}

	private void OnDestroy () {
		instance = null;
	}
}