using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceres.Entities {

	public class BodyPart : MonoBehaviour {

		private Body parentBody;
		private Rigidbody localPhysics;


		public Transform DollBone {
			get {
				return dollBone;
			}
			set {
				foreach (Transform child in value) {
					if (child.TryGetComponent(out CharacterJoint joint)) connectedJoints.Add(joint);
				}
				dollBone = value;
			}
		}

		private Transform dollBone;

		public GameObject dollRenderer;
		private List<CharacterJoint> connectedJoints;

		[SerializeField]
		private Rigidbody[] giblets;

		[SerializeField]
		private float gibEjectForce;

		private void Awake () {
			connectedJoints = new List<CharacterJoint>();

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

				foreach (CharacterJoint joint in connectedJoints) {
					Transform obj = joint.transform;
					//Destroy(joint);
					obj.SetParent(null, true);
				}

				Destroy(dollBone.gameObject);
				Destroy(dollRenderer);
				parentBody.Health -= damage;
				Destroy(gameObject);
			}
		}

		private void Update () {
			if (dollBone != null) dollBone.SetPositionAndRotation(transform.position, transform.rotation);
		}
	}
}