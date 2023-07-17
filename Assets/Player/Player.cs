using UnityEngine;
using UnityEngine.InputSystem;

namespace Ceres.Player {

	public class Player : MonoBehaviour {

		private static Player instance;

		[Header("Movement")]

		[SerializeField]
		private float moveSpeed;

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

		[SerializeField]
		private float speed;

		public static Vector3 Motion {
			get {
				return instance.motion;
			}
		}

		public static Rigidbody Physics {
			get {
				return instance.physics;
			}
		}

		private Rigidbody physics;

		private Vector2 moveInput;
		private Vector3 slopeMoveDirection;

		[Header("Camera")]
		//[SerializeField]
		//private InputActionReference lookAction;

		[SerializeField]
		private Camera view;

		public static Camera ViewPort {
			get {
				return instance.view;
			}
		}

		public static Transform Orientation {
			get {
				return instance.view.transform;
			}
		}

		[SerializeField]
		private Vector2 sensitivity;

		[SerializeField]
		private Transform cameraPos;

		//Horizontal rotation
		//Put the model here
		[SerializeField]
		private Transform rotation;

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

		public static bool OnSlope {
			get {
				return instance.onSlope;
			}
		}

		public static bool Grounded {
			get {
				return instance.grounded;
			}
		}

		private bool onSlope;
		private bool grounded;

		[Header("Drag")]
		[SerializeField]
		private float groundDrag;
		[SerializeField]
		private float airDrag;
		[SerializeField]
		private float slideDrag;

		[Header("Grappling Hook")]
		[SerializeField]
		private float grappleAcceleration;

		[SerializeField]
		private GameObject hookPrefab;

		private bool grappling;
		private Transform grapplePoint;
		[SerializeField]
		private float distance;

		[Header("Crouching")]
		//How much time it takes to move between crouched and not crouched states
		[SerializeField]
		private float crouchSpeed;
		[SerializeField]
		private Transform crouchedCenter;
		[SerializeField]
		private float slideForce;

		private bool crouching;
		private bool sliding;
		private int crouchDirection;

		[Header("Weapons")]
		[SerializeField] private GameObject currentWeapon;
		[SerializeField] private GameObject[] weapons;

		private void Awake () {
			instance = this;
			physics = GetComponent<Rigidbody>();
			physics.freezeRotation = true;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		private void Update () {
			grounded = UnityEngine.Physics.CheckSphere(groundCheck.position, groundDistance, worldLayer);
			onSlope = UnityEngine.Physics.Raycast(transform.position, Vector3.down, out slopeHit, height / 2 + 0.5f) && (slopeHit.normal != Vector3.up);

			ControlDrag();
			ControlFriction();
		}

		private void FixedUpdate () {
			UpdateCrouchState();
			UpdateRotation();
			UpdateMotion();
		}

		private void UpdateCrouchState () {
			if (speed < 5f) sliding = false;
			if (crouching) {
				float change = crouchSpeed * Time.fixedDeltaTime;
				Vector3 currentCenter = bounds.center;

				if (crouchDirection == 1) {
					currentCenter.y -= change;
					currentCenter.y = Mathf.Max(currentCenter.y, -0.5f);
				}
				else if (crouchDirection == -1) {
					currentCenter.y += change;
					currentCenter.y = Mathf.Min(currentCenter.y, 0.5f);
				}

				bounds.height = Mathf.Max(bounds.height - change, 1f);
				bounds.center = currentCenter;
				cameraPos.localPosition = currentCenter;
			}
			else {
				float change = crouchSpeed * Time.fixedDeltaTime;
				Vector3 currentCenter = bounds.center;

				if (grounded) {
					currentCenter.y += change;
					currentCenter.y = Mathf.Min(currentCenter.y, 0f);
				}
				else {
					currentCenter.y -= change;
					currentCenter.y = Mathf.Max(currentCenter.y, 0f);
				}

				bounds.height = Mathf.Min(bounds.height + change, 2f);
				bounds.center = currentCenter;
				cameraPos.localPosition = currentCenter;
			}

			groundCheck.localPosition = new Vector3(0, bounds.center.y - bounds.height / 2f, 0);
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
			rotation.rotation = Quaternion.Euler(0, horizontalAngle, 0);
			view.transform.localRotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
		}

		private void UpdateMotion () {
			Vector3 forwardsMotion = sliding ? Vector3.zero : rotation.forward * moveInput.y;
			Vector3 horizontalMotion = sliding ? Vector3.zero : rotation.right * moveInput.x;

			slopeMoveDirection = Vector3.ProjectOnPlane(forwardsMotion + horizontalMotion, slopeHit.normal);

			if (sliding) slopeMoveDirection = Vector3.ProjectOnPlane(rotation.forward, slopeHit.normal);

			Vector3 motionDirection = (forwardsMotion + horizontalMotion).normalized;
			float motionSpeed = moveSpeed * movementMultiplier;

			if (grounded && onSlope) {
				motionDirection = slopeMoveDirection.normalized;
			}

			if (!grounded) {
				motionSpeed *= airMultiplier;
			}

			direction = motionDirection;

			motion = direction * motionSpeed;

			if (grappling) {
				motion = ApplyGrappleMotion(motion);
			}

			physics.AddForce(motion, ForceMode.Acceleration);

			speed = physics.velocity.magnitude;
		}

		private Vector3 ApplyGrappleMotion (Vector3 motion) {
			Vector3 grappleDirection = grapplePoint.transform.position - transform.position;
			float distanceToAnchor = grappleDirection.magnitude;

			if (distance < distanceToAnchor) {
				float velocity = physics.velocity.magnitude;
				Vector3 newDirection = Vector3.ProjectOnPlane(physics.velocity, grappleDirection);
				physics.velocity = newDirection.normalized * velocity;
			}
			else physics.AddForce(grappleDirection.normalized * grappleAcceleration, ForceMode.Acceleration);

			motion += grappleDirection.normalized * grappleAcceleration;

			return motion;
		}

		private void ControlDrag () {
			if (grounded) {
				if (sliding) {
					physics.drag = slideDrag;
				}
				else physics.drag = groundDrag;
			}
			else physics.drag = airDrag;
		}

		private void ControlFriction () {
			if (grounded && onSlope && !sliding) {
				bounds.material.staticFriction = slopeFriction;
			}
			else {
				bounds.material.staticFriction = 0f;
			}
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
				Ray ray = new(transform.position, view.transform.forward);

				if (UnityEngine.Physics.Raycast(ray, out RaycastHit hit, 100f, worldLayer)) {
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
				physics.velocity = new Vector3(physics.velocity.x, 0, physics.velocity.z);
				physics.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
			}
		}

		public void Crouch (InputAction.CallbackContext context) {
			if (context.performed) {
				if (grounded && (onSlope || speed >= 15f)) {
					sliding = true;
					physics.AddForce(physics.velocity.normalized * slideForce, ForceMode.Impulse);
				}
				if (grounded) crouchDirection = -1;
				else crouchDirection = 1;
				crouching = true;
			}

			if (context.canceled) {
				crouchDirection = 0;
				crouching = false;
				sliding = false;
			}
		}

		public void Switch (InputAction.CallbackContext context) {
			if (context.performed) {
				Vector2 value = context.ReadValue<Vector2>();
				
				if (value.Equals(Vector2.up)) currentWeapon = weapons[0];
				else if (value.Equals(Vector2.right)) currentWeapon = weapons[1];
				else if (value.Equals(Vector2.down)) currentWeapon = weapons[2];
				else if (value.Equals(Vector2.left)) currentWeapon = weapons[3];
			}
		}

		private void OnDestroy () {
			instance = null;
		}
	}
}