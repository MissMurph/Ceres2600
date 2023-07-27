using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceres.Entities {

    public class Body : MonoBehaviour {

		public int Health {
			get {
				return health;
			}
			set {
				if (health + value <= 0) Die();
				health = value;
			}
		}

		[SerializeField]
		private int health;

		[SerializeField]
		private int maxHealth;

		private Dictionary<string, BodyPart> registeredLimbs;
		private Dictionary<string, GameObject> registeredRenderers;
		private Dictionary<string, Transform> registeredDollLimbs;

		[SerializeField]
		private LimbBinding[] limbs;

		[SerializeField]
		private GameObject doll;

		private void Awake () {
			registeredLimbs = new Dictionary<string, BodyPart>();
			registeredRenderers = new Dictionary<string, GameObject>();
			registeredDollLimbs = new Dictionary<string, Transform>();

			foreach (LimbBinding binding in limbs) {
				registeredLimbs[binding.limb.name] = binding.limb;
				registeredRenderers[binding.limb.name] = binding.renderer;

				string limbPath = binding.limb.transform.GetPath();
				Transform foundDoll = doll.transform.Find(limbPath);
				GameObject dollRenderer = doll.transform.Find(binding.renderer.name).gameObject;

				if (foundDoll != null) {
					registeredDollLimbs[binding.limb.name] = foundDoll;
					binding.limb.DollBone = registeredDollLimbs[binding.limb.name];
					binding.limb.dollRenderer = dollRenderer;
				}
				else Debug.Log("Couldn't find Ragdoll Bone " + limbPath);
			}

			health = maxHealth;
		}

		private void Update () {
			
		}

		private void Die () {
			doll.transform.parent = null;
			doll.SetActive(true);
			Destroy(gameObject);
		}
	}

	[Serializable]
	public class LimbBinding {
		public BodyPart limb;
		public GameObject renderer;
	}

	public static class TransformExtensions {
		public static string GetPath (this Transform current) {
			if (current.parent == null || current.name == "Armature")
				return current.name;
			return current.parent.GetPath() + "/" + current.name;
		}
	}
}