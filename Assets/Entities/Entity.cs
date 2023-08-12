using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceres.Entities {

    public class Entity : MonoBehaviour {

		[Header("Registration")]
		[SerializeField] private string key;
		[SerializeField] private int instanceId = -1;

		public string Registration {
			get {
				return key + ":" + instanceId;
			}
		}

		public int Id {
			get {
				return instanceId;
			}
		}

		public bool Initialized {
			get {
				return instanceId != -1;
			}
		}

        [Header("Health")]
        [SerializeField] private int maxHealth;
		[SerializeField] private int currentHealth;

		public int Health {
			get {
				return currentHealth;
			}
			set {
				//if (currentHealth + value <= 0) avatar.Die();
				currentHealth = value;
			}
		}

		//[Header("Avatar")]
		private Body avatar;

		public Body Avatar {
			get {
				return avatar;
			}
		}

		private void Awake () {
			avatar = GetComponent<Body>();

			currentHealth = maxHealth;

			if (Cache.Initialized) {
				int id = Cache.Register(this);

				if (id != -1) {
					key = name;
					instanceId = id;
				}
			}
			else {
				StartCoroutine(RequestRegistration());
			}
		}

		private IEnumerator RequestRegistration () {
			int id = -1;

			while (id == -1) {
				if (Cache.Initialized) {
					id = Cache.Register(this);

					if (id != -1) {
						key = name;
						instanceId = id;
						yield break;
					}
				}
				else yield return new WaitForSeconds(0.2f);
			}
		}

		public void Attack (float damage) {

        }
	}
}