using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour {

    public MidiController controller;
    public MIDIPlayer player;
    private float mouvement;
    private Transform metronome;
    private bool rightDir;



	// Use this for initialization
	void Start () {
        metronome = transform.GetChild(0);
        rightDir = true;
	}
	
	// Update is called once per frame
	void Update () {

        //Calcule la vitesse du metronome selon la velocity
        mouvement = player.currentVelocity / 360f;

        //Empeche le metronome de depasser
        if (mouvement + metronome.localPosition.x > 0.45f && 0.45f - metronome.localPosition.x > 0.01f)
            mouvement = 0.45f - metronome.localPosition.x;
        if (metronome.localPosition.x - mouvement < -0.45f && 0.45f + metronome.localPosition.x > 0.01f)
            mouvement = 0.45f + metronome.localPosition.x;

        //Change la direction du metronome aux limites
        if (metronome.localPosition.x >= 0.45f)
            rightDir = false;
        if (metronome.localPosition.x <= -0.45f)
            rightDir = true;

        //Bouge le metronome
        if (rightDir)
            metronome.Translate(mouvement, 0, 0);
        else
            metronome.Translate(-mouvement, 0, 0);
	}
}
