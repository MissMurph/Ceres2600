using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

namespace Ceres.Guns {

    public class GunLibrary : MonoBehaviour {
		private static GunLibrary instance;

		private Dictionary<string, GameObject> registeredPrefabs;
		private Dictionary<string, Weapon> registeredWeapons;

		[SerializeField]
		private GameObject[] prefabsToRegister;

		private void Awake () {
			instance = this;
			registeredPrefabs = new Dictionary<string, GameObject>();
			registeredWeapons = new Dictionary<string, Weapon>();

			foreach (GameObject prefab in prefabsToRegister) {
				Register(prefab.name, prefab);
			}
		}

		private void Register (string key, GameObject prefab) {
			if (registeredPrefabs.ContainsKey(key)) {
				throw new ArgumentException("Weapon " + key + " already registered!");
			}
			else {
				Weapon component = prefab.GetComponent<Weapon>();
				registeredPrefabs.Add(key, prefab);
				registeredWeapons.Add(key, component);
			}
		}

		public static GameObject Prefab (string key) {
			if (instance.registeredPrefabs.TryGetValue(key, out GameObject prefab)) {
				return prefab;
			}
			else throw new ArgumentException("Weapon " + key + " not registered!");
		}

		public static Weapon Unit (string key) {
			if (instance.registeredWeapons.TryGetValue(key, out Weapon component)) {
				return component;
			}
			else throw new ArgumentException("Weapon " + key + " not registered!");
		}
	}
}