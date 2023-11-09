using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceres.Spin {

    public class Spin : MonoBehaviour {

        [SerializeField]
        private float degreesPerSecond;

        [SerializeField]
        private float xAngle;

        private float angle;

		private void Awake () {
            angle = 0f;
		}

		void Update () {
            angle += degreesPerSecond * Time.deltaTime;
            transform.rotation = Quaternion.Euler(xAngle, angle, 0f);
        }
    }
}