using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGripIK : MonoBehaviour {

	private Animator pose;

	public Transform forwardGrip;
	public Transform backGrip;

	private void Awake () {
		pose = GetComponent<Animator>();
	}

	private void OnAnimatorIK () {
		if (pose && backGrip != null && forwardGrip != null) {
			/*	Right Hand	*/
			pose.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
			pose.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

			pose.SetIKPosition(AvatarIKGoal.RightHand, backGrip.position);
			pose.SetIKRotation(AvatarIKGoal.RightHand, backGrip.rotation);

			/*	Left Hand	*/
			pose.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
			pose.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

			pose.SetIKPosition(AvatarIKGoal.LeftHand, forwardGrip.position);
			pose.SetIKRotation(AvatarIKGoal.LeftHand, forwardGrip.rotation);
		}
		else {
			pose.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
			pose.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);

			pose.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
			pose.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);

			pose.SetLookAtWeight(0);
		}
	}
}
