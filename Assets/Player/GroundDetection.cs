using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceres.Player {

	public class GroundDetection : MonoBehaviour {

		[SerializeField]
		private LayerMask worldLayer;

		[SerializeField]
		private float groundDistance;

		[SerializeField]
		private float height;

		[SerializeField]
		private CapsuleCollider bounds;

		[SerializeField]
		private Transform groundCheck;

		private RaycastHit slope;

		public bool OnSlope {
			get {
				return onSlope;
			}
		}

		public bool Grounded {
			get {
				return grounded;
			}
		}

		public CapsuleCollider Bounds {
			get {
				return bounds;
			}
		}

		public LayerMask WorldLayer {
			get { 
				return worldLayer; 
			} 
		}

		public RaycastHit Slope {
			get {
				return slope;
			}
		}

		private bool onSlope;
		private bool grounded;

		private void Update () {
			grounded = Physics.CheckSphere(groundCheck.position, groundDistance, worldLayer);
			onSlope = Physics.Raycast(transform.position, Vector3.down, out slope, height / 2 + 0.5f);
		}
	}
}