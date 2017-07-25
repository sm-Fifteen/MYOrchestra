using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MenuButton : MonoBehaviour {
	private Text songNameUI;
	private Text songComposerUI;

	public string midiPath = "";
	public string songName = "";
	public string songComposer = "";

	void Awake () {
		foreach (Text textElement in GetComponentsInChildren<Text>()) {
			if (textElement.CompareTag ("SongName")) {
				songNameUI = textElement;
			} else if (textElement.CompareTag ("ArtistName")) {
				songComposerUI = textElement;
			}
		}
	}

	// Use this for initialization
	void OnValidate () {
		songNameUI.text = songName;
		songComposerUI.text = songComposer;
	}
}
