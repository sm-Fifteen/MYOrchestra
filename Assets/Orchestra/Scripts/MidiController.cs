using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pose = Thalmic.Myo.Pose;

public class MidiController : MonoBehaviour {
	public const float DIR_CHANGE_ANGLE_THRESH = 50; //Degrees
	public const float DIR_CHANGE_SPEED_THRESH = 60; //Degrees per sec

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
	private Vector2 lastDirection = Vector2.zero;
	private bool alreadyPausing = false;


	// Use this for initialization
	void Start () {
		maxMagnitude = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (thalmicMyo.arm == Thalmic.Myo.Arm.Unknown) return; // Ignore input if myo is not being worn

		MIDIPlayer player = GetComponent<MIDIPlayer>();
		Vector2 gyroXY = (Vector2) thalmicMyo.gyroscope;
		bool foo = false;

		// Has direction changed or movement paused?
		float angleDiff = Vector2.Angle(gyroXY.normalized, lastDirection);

		if (gyroXY.magnitude > DIR_CHANGE_SPEED_THRESH) {
			// Movement is fast enough to be recognizable
			if (angleDiff > DIR_CHANGE_ANGLE_THRESH) {
				// Angle has changed drastically without significant pause
				Debug.Log("Angle change");
				foo = true;
			} else {
				// Regular movement
				maxMagnitude = Mathf.Max(maxMagnitude, gyroXY.magnitude);
				alreadyPausing = false;
			}
		} else if (!alreadyPausing) {
			// Movement has stopped (and wasn't stopping before)
			Debug.Log("Paused");
			alreadyPausing = true;
			foo = true;
		}

		if (foo) {
			if (lastTime > 0) {
				player.currentTempo = (uint)(60 / (Time.time - lastTime));
			}

			thalmicMyo.Vibrate (Thalmic.Myo.VibrationType.Short);

			lastTime = Time.time;
			maxMagnitude = 0;
		}

		lastDirection = gyroXY.normalized;

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
}
