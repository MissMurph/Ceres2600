using Ceres.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ceres.Guns {

	public class MachineGun : Weapon {

		[Header("Input")]
		[SerializeField] private InputActionReference shootAction;
		[SerializeField] private InputActionReference reloadAction;

		[SerializeField] private int damage;

		//How many bullets per second can be fired from this gun
		[SerializeField] private float fireRate;

		protected float fireDelay;
		protected float cooldownTimer;

		//This bool determines if the gun is ready to fire or not
		//This will check if:
		//The gun is being reloaded
		//The magazine has ammo in it
		//The firing cooldown has lapsed
		protected bool Chambered {
			get {
				return !isReloading && CurrentAmmo > 0 && cooldownTimer >= fireDelay;
			}
		}

		[SerializeField] private float Recoil;

		[SerializeField] private float Accuracy;

		[SerializeField] private int MagSize;
		[SerializeField] private int CurrentAmmo;

		[SerializeField] private float ReloadTime;
		protected bool isReloading;

		private bool firing;

		[SerializeField]
		private Transform muzzlePos;

		private void Start () {
			AttachInput(OnShoot, shootAction);
			AttachInput(OnReload, reloadAction);
		}

		protected virtual void FireRay (Action<bool, Entity> callback) {
			Debug.DrawRay(muzzlePos.position, transform.forward * 100f, Color.yellow, 1f);

			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit) && hit.collider.gameObject.TryGetComponent(out Entity e)) {
				callback(true, e);
			}
			else callback(false, null);
		}

		protected virtual void Awake () {
			fireDelay = 1f / fireRate;
			cooldownTimer = fireDelay;
		}

		protected virtual void Update () {
			if (cooldownTimer < fireDelay) cooldownTimer += Time.deltaTime;

			if (Chambered && firing) {
				FireRay(OnHit);
				cooldownTimer -= fireDelay;
				CurrentAmmo--;
			}
		}

		private void OnShoot (InputAction.CallbackContext context) {
			if (context.performed) {
				firing = true;
			}

			if (context.canceled) {
				firing = false;
			}
		}

		private void OnReload (InputAction.CallbackContext context) {
			if (context.performed) {

			}
		}

		private void OnHit (bool result, Entity target) {
			if (result) target.Attack(damage);
		}

		protected void AttachInput (Action<InputAction.CallbackContext> listener, InputActionReference action) {
			action.action.started += listener;
			action.action.performed += listener;
			action.action.canceled += listener;
		}

		protected void DetachInput (Action<InputAction.CallbackContext> listener, InputActionReference action) {
			action.action.started -= listener;
			action.action.performed -= listener;
			action.action.canceled -= listener;
		}

		private void OnDestroy () {
			DetachInput(OnShoot, shootAction);
			DetachInput(OnReload, reloadAction);
		}
	}
}