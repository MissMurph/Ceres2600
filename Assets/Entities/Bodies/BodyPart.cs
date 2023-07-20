using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceres.Entities {

	public class BodyPart : MonoBehaviour {

		private Body parentBody;
		private Rigidbody localBody;

		private void Awake () {
			parentBody = GetComponentInParent<Body>();
			localBody = GetComponent<Rigidbody>();
		}

		public void Attack (Vector3 hit, int damage) {
			//We first need to determine if this is a killing hit
			if (parentBody.Health - damage <= 0) {

			}
		}
	}
}