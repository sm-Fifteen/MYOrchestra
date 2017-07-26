using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour {

	//public GameObject loadingImage;
	private static FileInfo songFile;

	public static void setSongFile(FileInfo file) {
		songFile = file;
	}

	public static FileInfo getSongFile() {
		return songFile;
	}

    public static void LoadScene(string level)
    {
        //loadingImage.SetActive(true);
        SceneManager.LoadScene(level);
    }
}
