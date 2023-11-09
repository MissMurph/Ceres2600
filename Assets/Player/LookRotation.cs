using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ceres.Player {

	public class LookRotation : MonoBehaviour {

		[SerializeField]
		private Camera view;

		public Camera ViewPort {
			get {
				return view;
			}
		}

		public Transform Orientation {
			get {
				return view.transform;
			}
		}

		public Transform Rotation {
			get {
				return rotation;
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

		private void Update () {
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

		public void Look (InputAction.CallbackContext context) {
			lookDelta = context.ReadValue<Vector2>();
		}
	}
}