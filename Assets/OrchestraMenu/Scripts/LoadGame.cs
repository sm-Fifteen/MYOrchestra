using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour {

	//public GameObject loadingImage;
	private static string songPath = "";

	public static void setSongPath(string filePath) {
		songPath = filePath;
	}

	public static string getSongPath() {
		return songPath;
	}

    public static void LoadScene(string level)
    {
        //loadingImage.SetActive(true);
        SceneManager.LoadScene(level);
    }
}
