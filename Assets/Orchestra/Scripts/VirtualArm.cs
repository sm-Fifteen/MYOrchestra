using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualArm : MonoBehaviour {
	public ThalmicMyo myo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Transform> ().rotation = myo.transform.localRotation;
	}
}
