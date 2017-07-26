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

	public FileInfo midiPath;
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
		if (midiPath == null) {
			Debug.LogError ("No song is mapped to that button");
			return;
		} else if (!midiPath.Exists) {
			Debug.LogError ("Song does not exist");
		}
			
		LoadGame.setSongFile(midiPath);
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
