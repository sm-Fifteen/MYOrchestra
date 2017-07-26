using System.Collections;
using System.Collections.Generic;
using System.IO;
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

	void Start() {
		GetComponent<Button> ().onClick.AddListener (LoadSong);
	}

	void Reset () {
		foreach (Text textElement in GetComponentsInChildren<Text>()) {
			if (textElement.CompareTag ("SongName")) {
				songNameUI = textElement;
			} else if (textElement.CompareTag ("ArtistName")) {
				songComposerUI = textElement;
			}
		}
	}

	void LoadSong() {
		if (midiPath == "") {
			Debug.LogError ("No song is mapped to that button");
			return;
		} else if (!File.Exists(midiPath)) {
			Debug.LogError ("Song does not exist");
			return;
		}
			
		LoadGame.setSongPath(midiPath);
		LoadGame.LoadScene("Orchestra/Main Scene");
	}

	// Use this for initialization
	void OnValidate () {
		updateText ();
	}

	private void updateText() {
		if (!songComposerUI || !songNameUI) Reset ();
		songNameUI.text = songName;
		songComposerUI.text = songComposer;
	}

	public void getMetadataFromFile(FileInfo file) {
		string fileName = Path.GetFileNameWithoutExtension (file.Name);
		string[] pathParts = fileName.Split(new string[]{"_-_"}, System.StringSplitOptions.RemoveEmptyEntries);

		if (pathParts.Length == 2) {
			songComposer = pathParts [0].Replace("_", " ");
			songName = pathParts [1].Replace("_", " ");
		} else {
			songName = pathParts [0].Replace("_", " ");
		}

		updateText ();
	}
}
