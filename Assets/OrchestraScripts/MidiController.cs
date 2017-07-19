using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiController : MonoBehaviour {


	private bool mouseWasDown = false;
	private float lastTime = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		MIDIPlayer player = GetComponent<MIDIPlayer>();

		if (Time.time - lastTime > 30) {
			lastTime = 0;
			mouseWasDown = false;
		}
		/*FOR NOW: holding t to change tempo, holding v to change volume
		tempo works by moving the mouse up and down like conductors (Chef d'orchestre ) */
		if(Input.GetKey("t")){

			if(Input.GetAxis("Mouse Y")<0){
				mouseWasDown = true;
			}
			if(Input.GetAxis("Mouse Y")>0){
				if (mouseWasDown) {
					if (lastTime>0) {
						player.currentTempo = (uint)(60/(Time.time - lastTime));
						print ((uint)(Time.time - lastTime));
					}
					lastTime = Time.time;
					mouseWasDown = false;
				}
			}
		}
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

	}


}
