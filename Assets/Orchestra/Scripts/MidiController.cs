﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Pose = Thalmic.Myo.Pose;

public class MidiController : MonoBehaviour {
	public const float START_MOVEMENT_SPEED_THRESH = 60; //Degrees per sec

	public const float MINIMUM_MOVEMENT_TIME = 0.2f;
	public const float LONG_PAUSE_MINIMUM_DURATION = 0.3f;
	public const float VELOCITY_TO_MAGNITUDE_SCALE = 200f;

	public ThalmicMyo thalmicMyo;
	private int beatCounter = 0;
	public MovementDirection expectedMovement;
	public UIPanel uiPanel;

	private float lastTime = 0.0f;

	private float magnitudeSum;
	private int magnitudeCount;
	private bool movingInTheRightDirection = false;

	public delegate void DownMovement ();
	public static event DownMovement onDownMovement;


	// Use this for initialization
	void Start () {
		string midiFile = LoadGame.getSongPath ();
		MIDIPlayer player = GetComponent<MIDIPlayer> ();
		player.enabled = false;
		if (midiFile != "") player.midiFilePath = midiFile;
		player.enabled = true;

		magnitudeSum = 0;
		magnitudeCount = 0;
		expectedMovement = getMovementDirection(beatCounter);
	}
	
	// Update is called once per frame
	void Update () {
		if (thalmicMyo.arm == Thalmic.Myo.Arm.Unknown) return; // Ignore input if myo is not being worn
		uiPanel.setArm(thalmicMyo.arm);

		MIDIPlayer player = GetComponent<MIDIPlayer>();
		Vector2 gyroXY = (Vector2) thalmicMyo.gyroscope;
		Vector2 expectedMovementVector = movementToVector (expectedMovement, thalmicMyo.arm);
		float deltaTime = Time.time - lastTime;

		// We expect a movement in a certain direction (supposing a 4:4 time signature)
		// We look for a movement down/left/right/up (in that order) and register a beat when that movement ends
		// (That is, when the vector component is no longer present)

		float componentValue = Vector2.Dot (gyroXY, expectedMovementVector);

		if (Mathf.Abs (Vector2.Dot (gyroXY.normalized, expectedMovementVector)) < 0.5) {
			// To avoid false positives, we ignore readings where the target component is not the largest of the two
			// Nothing, early exit
		} else if (movingInTheRightDirection) {
			if (componentValue < 0 && deltaTime > MINIMUM_MOVEMENT_TIME) {
				// No longer moving in the right direction
				movingInTheRightDirection = false;
				updateTempo ();

				float averageMagnitude = magnitudeSum / magnitudeCount;
				magnitudeSum = 0;
				magnitudeCount = 0;
				updateVelocity (averageMagnitude);
				Debug.Log (averageMagnitude);

				// So the Virtual arm can reset itself
				if (expectedMovement == MovementDirection.DOWN && onDownMovement != null) onDownMovement();

				beatCounter++;
				expectedMovement = getMovementDirection(beatCounter);
				uiPanel.showMovement(expectedMovement);
			} else {
				// Still moving in the right direction
				magnitudeSum += gyroXY.magnitude;
				magnitudeCount++;
			}
		} else {
			if (componentValue > START_MOVEMENT_SPEED_THRESH) {
				movingInTheRightDirection = true;
			}
		}
			
		/*
		if (deltaTime > LONG_PAUSE_MINIMUM_DURATION) {
			// Let the wind out of the orchestra
			// TODO : Pause immediately if hand is raised
			player.currentTempo = (uint)(60 / deltaTime);
		}
		*/
	}

	private static MovementDirection getMovementDirection(int movementIdx, uint movementsPerCycle = 4) {
		uint movementNum = (uint)(movementIdx % movementsPerCycle);

		switch (movementNum) {
			case 0: return MovementDirection.DOWN;
			case 1: return MovementDirection.INWARDS;
			case 2: return MovementDirection.OUTWARDS;
			case 3: return MovementDirection.UP;
			// Fallback case, I guess...
			default: return MovementDirection.DOWN;
		}
	}

	private static Vector2 movementToVector(MovementDirection movement, Thalmic.Myo.Arm conductingArm) {
		if (conductingArm == Thalmic.Myo.Arm.Unknown) return Vector2.zero;

		switch (movement) {
			case MovementDirection.DOWN:
				return Vector2.down;
			case MovementDirection.INWARDS:
				return (conductingArm == Thalmic.Myo.Arm.Right) ? Vector2.left : Vector2.right;
			case MovementDirection.OUTWARDS:
				return (conductingArm == Thalmic.Myo.Arm.Right) ? Vector2.right : Vector2.left;
			case MovementDirection.UP:
				return Vector2.up;
			default:
				return Vector2.zero;
		}
	}

	public enum MovementDirection {
		DOWN,
		INWARDS,
		OUTWARDS,
		UP
	}

	private uint updateTempo() {
		MIDIPlayer player = GetComponent<MIDIPlayer>();

		if (lastTime > 0) {
			//Debug.LogWarning (Time.time - lastTime);

			uint newTempo = (uint)(60 / (Time.time - lastTime));
			player.currentTempo = newTempo;
			thalmicMyo.Vibrate (Thalmic.Myo.VibrationType.Short);
		}

		lastTime = Time.time;
		return player.currentTempo;
	}

	private float updateVelocity(float averageMagnitude) {
		MIDIPlayer player = GetComponent<MIDIPlayer>();
		player.velocityScale = averageMagnitude / VELOCITY_TO_MAGNITUDE_SCALE;
		return player.velocityScale;
	}
}
