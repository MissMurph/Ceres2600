using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceres.Entities {

	public class BodyPart : MonoBehaviour {

		private Body parentBody;
		private Rigidbody localPhysics;

		[SerializeField]
		private Rigidbody[] giblets;

		[SerializeField]
		private float gibEjectForce;

		private void Awake () {
			parentBody = GetComponentInParent<Body>();
			localPhysics = GetComponent<Rigidbody>();
		}

		public void Attack (Vector3 hit, int damage) {
			System.Random rando = new System.Random();
			//We first need to determine if this is a killing hit
			if (parentBody.Health - damage <= 0) {
				foreach (Rigidbody gib in giblets) {
					gib.gameObject.SetActive(true);
					gib.AddForce(new Vector3(rando.Next(0,100), rando.Next(0, 100), rando.Next(0, 100)).normalized * gibEjectForce, ForceMode.Impulse);
				}

				parentBody.Health -= damage;
				Destroy(gameObject);
			}
		}
	}
}