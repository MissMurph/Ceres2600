using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ceres.Entities {

	public class BodyPart : MonoBehaviour {

		private Body parentBody;

		[Header("Ragdoll Link")]
		//The Ragdoll bone that moves with this bone
		[SerializeField] private Transform sisterBone;
		[SerializeField] private GameObject[] bones;
		[SerializeField] private GameObject skin;
		private Rigidbody physicsBody;

		public Rigidbody Physics {
			get { 
				return physicsBody;
			}
		}

		[Header("Neighbours")]
		[SerializeField] private BodyPart[] neighbours;
		[SerializeField] private Rigidbody[] jointGibs;
		private CharacterJoint jointComp;

		[Header("Giblets")]
		private List<Rigidbody> giblets;
		[SerializeField] private float gibEjectForce;

		private void Awake () {
			giblets = new();
			physicsBody = sisterBone.GetComponent<Rigidbody>();

			if (sisterBone.TryGetComponent(out CharacterJoint comp)) jointComp = comp;

			parentBody = transform.root.GetComponent<Body>();

			foreach (Transform child in sisterBone) {
				if (child.tag == "Giblet" && child.TryGetComponent(out Rigidbody gibPhysics)) {
					giblets.Add(gibPhysics);
				}
			}
		}

		public void Attack (Vector3 hit, int damage) {
			parentBody.AttackBody(this, damage);
		}

		public void Detach (BodyPart source) {
			if (jointComp != null 
				&& ReferenceEquals(jointComp.connectedBody, source.Physics)) 
				Component.Destroy(jointComp);

			for (int i = 0; i < jointGibs.Length; i++) {
				if (ReferenceEquals(neighbours[i], source)) {
					jointGibs[i].gameObject.SetActive(true);
				}
			}
		}

		private void Update () {
			sisterBone.SetPositionAndRotation(transform.position, transform.rotation);
		}

		public void Destruct () {
			System.Random rando = new();

			foreach (BodyPart joint in neighbours) {
				joint.Detach(this);
			}

			foreach (GameObject bone in bones) {
				foreach (Transform child in bone.transform) {
					if (!bones.Contains(child.gameObject)) child.SetParent(null, true);
				}
			} 

			foreach (GameObject bone in bones) {
				Destroy(bone);
			}

			Destroy(skin);

			foreach (Rigidbody gib in giblets) {
				gib.transform.SetParent(null, true);
				gib.gameObject.SetActive(true);
				gib.AddForce(new Vector3(rando.Next(0, 100), rando.Next(0, 100), rando.Next(0, 100)).normalized * gibEjectForce, ForceMode.Impulse);
			}
		}
	}
}