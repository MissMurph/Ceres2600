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
		private DollLimb[] limbs;

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

			foreach (DollLimb binding in limbs) {
				string path = binding.limb.transform.BonePath();
				Transform sisterBone = doll.transform.Find(path);

				//List<GameObject> dollBones = new();

				/*foreach (GameObject avatarBone in binding.bones) {
					Transform foundDollBone = doll.transform.Find(avatarBone.transform.BonePath());

					if (foundDollBone == null) throw new Exception("No matching ragdoll bone found for child-bone " + avatarBone.transform.BonePath() + "!");

					dollBones.Add(foundDollBone.gameObject);
				}*/

				if (sisterBone == null) throw new Exception("No matching ragdoll bone found for limb " + path + "!");

				registeredLimbs[binding.limb.name] = binding.limb;

				binding.limb.SisterBone = sisterBone;
				binding.limb.SisterSkin = binding.skin;
				binding.limb.BoneBundle = binding.bones;
				binding.limb.Joints = binding.linkedJoints;
				binding.limb.Parent = parent;
			}

			init = true;
		}

		public void Die () {
			doll.transform.parent = null;
			doll.SetActive(true);
			Destroy(gameObject);
		}

		//If there are no joints connected to a limb, this will return a new empty list
		private List<CharacterJoint> JointList (string parentPath) {
			List<CharacterJoint> joints = registeredJoints.GetValueOrDefault(parentPath, new List<CharacterJoint>());
			return joints;
		}
	}

	[Serializable]
	public class DollLimb {
		public BodyPart limb;
		public GameObject skin;
		public GameObject[] bones;
		public CharacterJoint[] linkedJoints;
	}

	internal static class TransformExtensions {
		internal static string BonePath (this Transform current) {
			if (current.parent == null || current.name == "Armature")
				return current.name;
			return current.parent.BonePath() + "/" + current.name;
		}
	}
}