using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

namespace Ceres.Entities {

    public class Body : MonoBehaviour {

		private Dictionary<string, BodyPart> registeredLimbs;
		private Dictionary<string, List<CharacterJoint>> registeredJoints;

		[SerializeField]
		private BodyPart[] limbs;

		[SerializeField]
		private GameObject doll;

		public bool Initialized {
			get {
				return init;
			}
		}

		private bool init = false;

		public BodyPart this[string path] {
			get {
				if (init && registeredLimbs.TryGetValue(path, out BodyPart found)) {
					return found;
				}
				else throw new ArgumentException("No matching limb found at " + path + "!");
			}
		}

		public GameObject Ragdoll {
			get {
				return doll;
			}
		}

		private Entity controller;

		private void Awake () {
			controller = GetComponent<Entity>();
			registeredLimbs = new Dictionary<string, BodyPart>();
			registeredJoints = new Dictionary<string, List<CharacterJoint>>();
		}

		public void AttackBody (BodyPart limbHit, int damage) {
			if (controller.Health - damage <= 0) {
				limbHit.Destruct();
				Die();
			}

			controller.Health -= damage;
		}

		private void Die () {
			doll.transform.parent = null;
			doll.SetActive(true);
			Destroy(gameObject);
		}
	}

	internal static class TransformExtensions {
		internal static string BonePath (this Transform current) {
			if (current.parent == null || current.name == "Armature")
				return current.name;
			return current.parent.BonePath() + "/" + current.name;
		}
	}
}