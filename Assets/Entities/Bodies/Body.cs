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
				if (health + value <= 0) ;
				health = value;
			}
		}

		private int health;

        private Dictionary<string, GameObject> registeredLimbs;



		private void Awake () {
			registeredLimbs = new Dictionary<string, GameObject>();

			foreach (GameObject child in transform) {
				registeredLimbs[child.name] = child;
			}
		}

		private void Ragdoll () {

		}
	}
}