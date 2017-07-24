using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MovementIndicator : MonoBehaviour {

	public Sprite iconMovementDown;
	public Sprite iconMovementInwards;
	public Sprite iconMovementOutwards;
	public Sprite iconMovementUp;
	public float imageScaling = 2;
	private Thalmic.Myo.Arm currentArm = Thalmic.Myo.Arm.Unknown;

	public void showMovement(MidiController.MovementDirection direction) {
		Image imageComponent = GetComponent<Image> ();

		switch(direction){
			case MidiController.MovementDirection.DOWN:
				imageComponent.sprite = iconMovementDown;
				break;
			case MidiController.MovementDirection.INWARDS:
				imageComponent.sprite = iconMovementInwards;
				break;
			case MidiController.MovementDirection.OUTWARDS:
				imageComponent.sprite = iconMovementOutwards;
				break;
			case MidiController.MovementDirection.UP:
				imageComponent.sprite = iconMovementUp;
				break;
		}
	}

	public void setArm(Thalmic.Myo.Arm conductingArm) {
		if (conductingArm == currentArm) return;
		currentArm = conductingArm;

		RectTransform transform = GetComponent<RectTransform> ();

		if (conductingArm == Thalmic.Myo.Arm.Left) {
			// Mirrored
			transform.localScale = imageScaling * new Vector2(-1f, 1f);
		} else {
			// Not mirrored
			transform.localScale = imageScaling * new Vector2(1f, 1f);
		}
	}
}
