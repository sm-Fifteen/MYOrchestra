using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Metronome : MonoBehaviour {

    public MidiController controller;
    public MIDIPlayer player;
	public Text bpmLabel;
	public string bpmLabelSuffix = "BPM";
    private float mouvement;
    private Scrollbar metronome;
    private int direction;



	// Use this for initialization
	void Start () {
		metronome = GetComponent<Scrollbar>();
		metronome.value = 0;
		direction = 1;
	}
	
	// Update is called once per frame
	void Update () {
		/*
        //Calcule la vitesse du metronome selon la velocity
        mouvement = player.currentVelocity / 360f;

        //Empeche le metronome de depasser
        if (mouvement + metronome.localPosition.x > 0.45f && 0.45f - metronome.localPosition.x > 0.01f)
            mouvement = 0.45f - metronome.localPosition.x;
        if (metronome.localPosition.x - mouvement < -0.45f && 0.45f + metronome.localPosition.x > 0.01f)
            mouvement = 0.45f + metronome.localPosition.x;
		*/

		if (bpmLabel != null) {
			bpmLabel.text = player.targetTempo + " " + bpmLabelSuffix;
		}

        //Change la direction du metronome aux limites
		if (metronome.value >= 1f)
			direction = -1;
		if (metronome.value <= 0f)
			direction = 1;

        //Bouge le metronome
		metronome.value += (Time.deltaTime/(60/player.targetTempo)) * direction;
	}
}
