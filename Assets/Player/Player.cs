using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

	[SerializeField]
	private float maxMoveSpeed;

	[SerializeField]
	private float acceleration;

	[SerializeField]
	private Vector2 moveDirection;

	//This will be the delta of the mouse, how quickly we need to turn
	//x delta needs to be rotated around the y axis
	//y delta needs to be rotated around the x axis, clamped between -90 & 90
	[SerializeField]
	private Vector2 lookDelta;

	private float verticalAngle;
	private float horizontalAngle;

	[SerializeField]
	private Vector2 sensitivity;

	[SerializeField]
	private float jumpForce;

	private Rigidbody body;
	private Rigidbody cameraBody;

	private Camera view;

	[SerializeField]
	private Vector2 currentVelocity;

	private void Awake () {
		body = GetComponent<Rigidbody>();
		cameraBody = GetComponentInChildren<Rigidbody>();
		view = GetComponentInChildren<Camera>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void FixedUpdate () {
		horizontalAngle += lookDelta.x * sensitivity.x;
		verticalAngle -= lookDelta.y * sensitivity.y;

		verticalAngle = Mathf.Clamp(verticalAngle, -90f, 90f);

		//body.MoveRotation();
		cameraBody.MoveRotation(Quaternion.Euler(verticalAngle, horizontalAngle, 0));

		//Get current velocity
		currentVelocity = body.velocity;

		//Add acceleration amount
		currentVelocity += acceleration * Time.fixedDeltaTime * moveDirection;

		//Clamp the new velocity so it doesn't exceed max speed
		currentVelocity.x = Mathf.Min(maxMoveSpeed, currentVelocity.x);
		currentVelocity.y = Mathf.Min(maxMoveSpeed, currentVelocity.y);

		body.velocity = new Vector3(currentVelocity.x, 0, currentVelocity.y);
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

	}

	public void Grab (InputAction.CallbackContext context) {

	}

	public void Reload (InputAction.CallbackContext context) {

	}

	public void Jump (InputAction.CallbackContext context) {
		if (context.canceled) {
			body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}
	}

	public void Crouch (InputAction.CallbackContext context) {

	}
}