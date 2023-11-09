using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceres.Entities {

    public class Cache : MonoBehaviour {

		private static Cache instance;

        private Dictionary<string, Dictionary<int, Entity>> instances;

		private int instanceIndex;

		public static int Index {
			get {
				return instance.instanceIndex;
			}
		}

		public static Cache Get {
			get {
				return instance;
			}
		}

		public static bool Initialized {
			get {
				return instance != null;
			}
		}

		public Entity this[string name] {
			get {
				string[] split = name.Split(':');
				string type = split[0];
				string id = split[1];

				Dictionary<int, Entity> map = instance.GetMap(type);

				return map[int.Parse(id)];
			}
		}

		private void Awake () {
			instance = this;
			instanceIndex = 0;
			instances = new Dictionary<string, Dictionary<int, Entity>>();
		}

		//Returns -1 for an unsuccessful register
		public static int Register (Entity unit) {
			Dictionary<int, Entity> map = instance.GetMap(unit.name);
			instance.instanceIndex++;
			int index = instance.instanceIndex;
			return map.TryAdd(index, unit) ? index : -1;
		}

		private Dictionary<int, Entity> GetMap (string key) {
			Dictionary<int, Entity> map = instances.GetValueOrDefault(key, new Dictionary<int, Entity>());
			if (!instances.ContainsKey(key)) instances.Add(key, map);
			return map;
		}

		private void OnDestroy () {
			instance = null;
		}
	}
}