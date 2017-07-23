using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pose = Thalmic.Myo.Pose;

public class MidiController : MonoBehaviour {
	public const float START_MOVEMENT_SPEED_THRESH = 60; //Degrees per sec

	public const float MINIMUM_MOVEMENT_TIME = 0.2f;
	public const float LONG_PAUSE_MINIMUM_DURATION = 0.3f;

	public ThalmicMyo thalmicMyo;
	public Vector2 expectedMovement;

	private float lastTime = 0.0f;

	private float maxMagnitude;
	private bool movingInTheRightDirection = false;


	// Use this for initialization
	void Start () {
		maxMagnitude = 0;
		expectedMovement = Vector2.down;
	}
	
	// Update is called once per frame
	void Update () {
		if (thalmicMyo.arm == Thalmic.Myo.Arm.Unknown) return; // Ignore input if myo is not being worn

		MIDIPlayer player = GetComponent<MIDIPlayer>();
		Vector2 gyroXY = (Vector2) thalmicMyo.gyroscope;

		// We expect a movement in a certain direction (supposing a 4:4 time signature)
		// We look for a movement down/left/right/up (in that order) and register a beat when that movement ends
		// (That is, when the vector component is no longer present)

		float componentValue = Vector2.Dot (gyroXY, expectedMovement);

		if (Mathf.Abs (Vector2.Dot (gyroXY.normalized, expectedMovement)) < 0.5) {
			// To avoid false positives, we ignore readings where the target component is not the largest of the two
			// Nothing, early exit
		} else if (movingInTheRightDirection) {
			if (componentValue < 0) {
				// No longer moving in the right direction
				movingInTheRightDirection = false;
				updateTempo();
				expectedMovement = nextMovement (thalmicMyo.arm);

				Debug.Log (gyroXY.ToString());
			} else {
				// Still moving in the right direction
				maxMagnitude = Mathf.Max(maxMagnitude, gyroXY.magnitude);
			}
		} else {
			if (componentValue > START_MOVEMENT_SPEED_THRESH) {
				movingInTheRightDirection = true;
			}
		}

		/*
		if ((Time.time - lastTime) > LONG_PAUSE_MINIMUM_DURATION) {
			// TODO : Pause immediately if hand is raised
			player.currentTempo = (uint)(60 / (Time.time - lastTime));
		}
		*/

		/*
		if (Input.GetKey ("v")) {

			if (Input.GetAxis ("Mouse X") < 0) {
				player.velocityScale = player.velocityScale - 0.02f;
				if (player.velocityScale < 0)
					player.velocityScale = 0;
			}
			if (Input.GetAxis ("Mouse X") > 0) {
				player.velocityScale = player.velocityScale + 0.02f;
				if (player.velocityScale > 2)
					player.velocityScale = 2;
			}
		}
		*/
	}

	private Vector2 nextMovement(Thalmic.Myo.Arm conductingArm) {
		if (expectedMovement == Vector2.up) {
			// Current is up (4th), next is down (1st)
			return Vector2.down;
		} else if (expectedMovement == Vector2.down && conductingArm == Thalmic.Myo.Arm.Right) {
			// (Right handed) Current is down (1st), next is inwards (2nd)
			return Vector2.left;
		} else if (expectedMovement == Vector2.down && conductingArm == Thalmic.Myo.Arm.Left) {
			// (Left handed) Current is down (1st), next is inwards (2nd)
			return Vector2.right;
		} else if (expectedMovement == Vector2.left && conductingArm == Thalmic.Myo.Arm.Right) {
			// (Right handed) Current is inwards (2nd), next is outwards (3rd)
			return Vector2.right;
		} else if (expectedMovement == Vector2.right && conductingArm == Thalmic.Myo.Arm.Left) {
			// (Left handed) Current is inwards (2nd), next is outwards (3rd)
			return Vector2.left;
		} else if (expectedMovement == Vector2.right && conductingArm == Thalmic.Myo.Arm.Right) {
			// (Right handed) Current is outwards (3rd), next is up (4th)
			return Vector2.up;
		} else if (expectedMovement == Vector2.left && conductingArm == Thalmic.Myo.Arm.Left) {
			// (Left handed) Current is outwards (3rd), next is up (4th)
			return Vector2.up;
		} else {
			// Fallback case, I guess...
			return Vector2.down;
		}
	}

	private uint updateTempo() {
		if ((Time.time - lastTime) < MINIMUM_MOVEMENT_TIME) return 0;
		MIDIPlayer player = GetComponent<MIDIPlayer>();

		if (lastTime > 0) {
			Debug.LogWarning (Time.time - lastTime);

			uint newTempo = (uint)(60 / (Time.time - lastTime));
			player.currentTempo = newTempo;
			thalmicMyo.Vibrate (Thalmic.Myo.VibrationType.Short);
		}

		lastTime = Time.time;
		return player.currentTempo;
	}
}
