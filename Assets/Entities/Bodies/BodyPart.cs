using System.Collections.Generic;
using UnityEngine;

namespace Ceres.Entities {

	public class BodyPart : MonoBehaviour {

		private Entity parentEntity;

		[Header("Ragdoll Link")]
		//The Ragdoll bone that moves with this bone
		[SerializeField] private Transform sisterBone;
		[SerializeField] private GameObject[] bones;
		[SerializeField] private GameObject skin;

		[Header("Neighbours")]
		[SerializeField] private BodyPart[] neighbours;
		[SerializeField] private Rigidbody[] jointGibs;
		private CharacterJoint jointComp;

		[Header("Giblets")]
		private List<Rigidbody> giblets;
		[SerializeField] private float gibEjectForce;

		private void Awake () {
			giblets = new();

			if (TryGetComponent(out CharacterJoint comp)) jointComp = comp;

			parentEntity = transform.root.GetComponent<Entity>();

			foreach (Transform child in transform) {
				if (child.tag == "Giblet" && child.TryGetComponent(out Rigidbody gibPhysics)) {
					giblets.Add(gibPhysics);
				}
			}
		}

		public void Attack (Vector3 hit, int damage) {
			//We first need to determine if this is a killing hit
			if (parentEntity.Health - damage <= 0) {
				Destroy(this);
			}

			parentEntity.Health -= damage;
		}

		public void Detach (BodyPart source) {
			if (jointComp != null) Destroy(jointComp);

			for (int i = 0; i < jointGibs.Length; i++) {
				if (ReferenceEquals(neighbours[i], source)) {
					jointGibs[i].gameObject.SetActive(true);
				}
			}
		}

		private void Update () {
			sisterBone.SetPositionAndRotation(transform.position, transform.rotation);
		}

		private void OnDestroy () {
			System.Random rando = new();

			foreach (GameObject bone in bones) {
				foreach (Transform child in bone.transform) {
					child.SetParent(null, true);
				}
			}

			foreach (BodyPart joint in neighbours) {
				joint.Detach(this);
			}

			foreach (Rigidbody gib in giblets) {
				gib.transform.SetParent(null, true);
				gib.gameObject.SetActive(true);
				gib.AddForce(new Vector3(rando.Next(0, 100), rando.Next(0, 100), rando.Next(0, 100)).normalized * gibEjectForce, ForceMode.Impulse);
			}

			Destroy(sisterBone);
			Destroy(skin);
		}
	}
}