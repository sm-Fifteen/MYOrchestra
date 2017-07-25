using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MenuButton : MonoBehaviour {
	private Text songNameUI;
	private Text songComposerUI;

	public string midiPath {
		get {
			return _midiPath;
		}
		set {
			string[] pathParts = value.Split(new string[]{"_-_"}, System.StringSplitOptions.RemoveEmptyEntries);
			switch (pathParts.Length) {
				case 2:
					songComposer = pathParts [1];
					goto case 1; // Fallthrough
				case 1:
					songName = pathParts [0];
					break;
			}
			_midiPath = value;
		}
	}
	private string _midiPath = "";
	public string songName = "";
	public string songComposer = "";

	void Reset () {
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
		if (!songComposerUI || !songNameUI) Reset ();
		songNameUI.text = songName;
		songComposerUI.text = songComposer;
	}
}
