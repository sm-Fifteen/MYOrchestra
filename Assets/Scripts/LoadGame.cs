using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour {

	//public GameObject loadingImage;

    public void LoadScene(string level)
    {
        //loadingImage.SetActive(true);
        SceneManager.LoadScene(level);
    }
}
