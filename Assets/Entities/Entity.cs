using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceres.Entities {

    public class Entity : MonoBehaviour {

        [Header("Health")]
        [SerializeField] private float maxHealth;
		[SerializeField] private float currentHealth;

        public void Attack (float damage) {

        }
	}
}