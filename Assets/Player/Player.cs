using Ceres.Guns;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ceres.Player {

	public class Player : MonoBehaviour {

		private static Player instance;

		[Header("Weapons")]
		[SerializeField] private MachineGun currentWeapon;
		[SerializeField] private GameObject[] weapons;

		private void Awake () {
			instance = this;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		public void OnShoot (InputAction.CallbackContext context) {
			//currentWeapon.OnShoot(context);
		}

		public void OnReload (InputAction.CallbackContext context) {
			//currentWeapon.OnReload(context);
		}

		public void Grab (InputAction.CallbackContext context) {

		}

		public void Switch (InputAction.CallbackContext context) {
			/*if (context.performed) {
				Vector2 value = context.ReadValue<Vector2>();
				
				if (value.Equals(Vector2.up)) currentWeapon = weapons[0];
				else if (value.Equals(Vector2.right)) currentWeapon = weapons[1];
				else if (value.Equals(Vector2.down)) currentWeapon = weapons[2];
				else if (value.Equals(Vector2.left)) currentWeapon = weapons[3];
			}*/
		}

		private void OnDestroy () {
			instance = null;
		}
	}
}