using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pose = Thalmic.Myo.Pose;

public class MidiController : MonoBehaviour {
	public const float START_MOVEMENT_SPEED_THRESH = 60; //Degrees per sec

	public const float MINIMUM_MOVEMENT_TIME = 0.2f;
	public const float LONG_PAUSE_MINIMUM_DURATION = 0.3f;

	public ThalmicMyo thalmicMyo;

	private bool mouseWasDown = false;
	private float lastTime = 0.0f;

	// A rotation that compensates for the Myo armband's orientation parallel to the ground, i.e. yaw.
	// Once set, the direction the Myo armband is facing becomes "forward" within the program.
	// Set by making the fingers spread pose or pressing "r".
	private Quaternion _antiYaw = Quaternion.identity;

	// A reference angle representing how the armband is rotated about the wearer's arm, i.e. roll.
	// Set by making the fingers spread pose or pressing "r".
	private float _referenceRoll = 0.0f;

	// The pose from the last update. This is used to determine if the pose has changed
	// so that actions are only performed upon making them rather than every frame during
	// which they are active.
	private Pose _lastPose = Pose.Unknown;

	private float maxMagnitude;
	private bool movingInTheRightDirection = false;
	public Vector2 expectedMovement;


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

		/*******************************************************/
		/*
		if (gyroXY.magnitude > MOVEMENT_SPEED_THRESH) {
            // Regular movement
            maxMagnitude = Mathf.Max(maxMagnitude, gyroXY.magnitude);
            alreadyPausing = false;
		} else if (!alreadyPausing) {
			// Movement has stopped (and wasn't stopping before)
			Debug.Log("Paused");
			alreadyPausing = true;
			updateTempo = true;
		}

		if (updateTempo && (Time.time - lastTime) > MINIMUM_MOVEMENT_TIME) {
			if (lastTime > 0) {
				player.currentTempo = (uint)(60 / (Time.time - lastTime));
				Debug.LogWarning (Time.time - lastTime);
			}

			thalmicMyo.Vibrate (Thalmic.Myo.VibrationType.Short);

			lastTime = Time.time;
			maxMagnitude = 0;
		} else if (alreadyPausing && (Time.time - lastTime) > LONG_PAUSE_MINIMUM_DURATION) {
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
