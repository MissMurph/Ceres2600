using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

		private void Awake () {
			registeredLimbs = new Dictionary<string, BodyPart>();
			registeredJoints = new Dictionary<string, List<CharacterJoint>>();
			Entity parent = GetComponent<Entity>();

			/*foreach (CharacterJoint joint in joints) {
				string parentPath = joint.connectedBody.transform.BonePath();

				List<CharacterJoint> collection = JointList(parentPath);
				collection.Add(joint);

				if (!registeredJoints.ContainsKey(parentPath)) registeredJoints[parentPath] = collection;
			}*/

			

			init = true;
		}

		public void Die () {
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