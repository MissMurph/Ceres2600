using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceres.Entities {

    public class Ragdoll : MonoBehaviour {

		private Dictionary<string, BodyPart> registeredLimbs;
		private Dictionary<string, GameObject> registeredRenderers;

		private LimbBinding[] limbs;
	}
}