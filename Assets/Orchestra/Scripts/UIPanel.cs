using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour {
	private Thalmic.Myo.Arm currentArm = Thalmic.Myo.Arm.Unknown;

	public void showMovement(MidiController.MovementDirection direction) {
		GetComponentInChildren<MovementIndicator>().showMovement(direction);
	}

	public void setArm(Thalmic.Myo.Arm conductingArm) {
		if (conductingArm == currentArm) return;
		currentArm = conductingArm;

		RectTransform transform = GetComponent<RectTransform> ();

		if (conductingArm == Thalmic.Myo.Arm.Left) {
			// UI aligned left
			transform.anchorMin = new Vector2 (0f, 0f);
			transform.anchorMax = new Vector2 (0f, 1f);
			transform.pivot = new Vector2 (0f, 0.5f);
		} else {
			// UI aligned right
			transform.anchorMin = new Vector2 (1f, 0f);
			transform.anchorMax = new Vector2 (1f, 1f);
			transform.pivot = new Vector2 (1f, 0.5f);
		}

		GetComponentInChildren<MovementIndicator>().setArm(conductingArm);
	}
}
