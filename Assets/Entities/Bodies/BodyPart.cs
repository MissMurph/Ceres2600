using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceres.Entities {

	public class BodyPart : MonoBehaviour {

		public Entity Parent {
			get {
				return parentEntity;
			}
			set {
				if (parentEntity != null) return;
				parentEntity = value;
			}
		}

		public Transform SisterBone {
			get {
				return sisterBone;
			}
			set {
				if (sisterBone != null) return;
				sisterBone = value;
			}
		}

		public GameObject SisterSkin {
			get {
				return skin;
			}
			set {
				if (skin != null) return;
				skin = value;
			}
		}

		public GameObject[] BoneBundle {
			get { 
				return bones; 
			}
			set {
				if (bones != null) return;
				bones = value;
			}
		}

		public CharacterJoint[] Joints {
			get {
				return connectedJoints;
			}
			set {
				if (connectedJoints != null) return;
				connectedJoints = value;
			}
		}

		public bool Initialized {
			get {
				return sisterBone != null
					&& bones != null
					&& skin != null
					&& connectedJoints != null;
			}
		}

		private Entity parentEntity;
		//The Ragdoll bone that moves with this bone
		private Transform sisterBone;
		private GameObject[] bones;
		private GameObject skin;
		private CharacterJoint[] connectedJoints;

		[SerializeField]
		private Rigidbody[] giblets;

		[SerializeField]
		private float gibEjectForce;

		public void Attack (Vector3 hit, int damage) {
			if (!Initialized) return;
			System.Random rando = new System.Random();

			//We first need to determine if this is a killing hit
			if (parentEntity.Health - damage <= 0) {
				DetachSister();
			}

			parentEntity.Health -= damage;
		}

		//This only detaches from the ragdoll, limbs will be destroyed anyways by the Body so we won't destroy the limb here.
		public void DetachSister () {
			System.Random rando = new();

			foreach (Rigidbody gib in giblets) {
				gib.transform.SetParent(null, true);
				gib.gameObject.SetActive(true);
				gib.AddForce(new Vector3(rando.Next(0,100), rando.Next(0, 100), rando.Next(0, 100)).normalized * gibEjectForce, ForceMode.Impulse);
			}

			foreach (CharacterJoint joint in connectedJoints) {
				Component.Destroy(joint);
			}

			foreach (GameObject bone in bones) {
				foreach (Transform child in bone.transform) {
					child.SetParent(null, true);
				}
			}

			Destroy(sisterBone.gameObject);
			Destroy(SisterSkin);
		}

		private void Update () {
			if (Initialized) sisterBone.SetPositionAndRotation(transform.position, transform.rotation);
		}
	}
}