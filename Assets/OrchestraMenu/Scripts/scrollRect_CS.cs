using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class scrollRect_CS : MonoBehaviour {
    public RectTransform center;
	public GameObject btnPrefab;
	[RangeAttribute(0, 1000)] public float btnDistance = 300;

    private bool dragging = false; //true = drag
    private int minButtonNum;

	void refreshButtonList() {
		foreach(Button button in GetComponentsInChildren<Button>()){
			DestroyImmediate(button.gameObject);
		}

		DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Music_menu/");
		FileInfo[] fichiers = dir.GetFiles();

		List<String> list = new List<String>();

		foreach (FileInfo fichier in fichiers) {
			if (fichier.Extension != ".mid") continue;

			GameObject btnObject = Instantiate(btnPrefab) as GameObject;
			btnObject.transform.SetParent(transform);

			btnObject.GetComponent<MenuButton>().midiPath = fichier.ToString();
			btnObject.GetComponent<MenuButton> ().getMetadataFromFile(fichier);
		}
	}

    void Start(){
		updateButtonDistance();
    }

	void OnValidate() {
		UnityEditor.EditorApplication.delayCall+=()=>{
			refreshButtonList ();
			updateButtonDistance();
		};
	}

    void Update(){
		Button[] btn = GetComponentsInChildren<Button> ();
		float[] distance = new float[btn.Length];
		float[] distanceRepo = new float[btn.Length];

        for (int i = 0; i < btn.Length; i++) {
            distanceRepo[i] = center.GetComponent<RectTransform>().position.x - btn[i].GetComponent<RectTransform>().position.x;
            distance[i] = Mathf.Abs(distanceRepo[i]);

            if (distanceRepo[i] > 1000){
                float curX = btn[i].GetComponent<RectTransform>().anchoredPosition.x;
                float curY = btn[i].GetComponent<RectTransform>().anchoredPosition.y;

				Vector2 newAnchoredPos = new Vector2(curX + (btn.Length * btnDistance), curY);
                btn[i].GetComponent<RectTransform>().anchoredPosition = newAnchoredPos;
            }

            if (distanceRepo[i] < -1000)
            {
                float curX = btn[i].GetComponent<RectTransform>().anchoredPosition.x;
                float curY = btn[i].GetComponent<RectTransform>().anchoredPosition.y;

				Vector2 newAnchoredPos = new Vector2(curX - (btn.Length * btnDistance), curY);
                btn[i].GetComponent<RectTransform>().anchoredPosition = newAnchoredPos;
            }
        }

        float minDistance = Mathf.Min(distance);

        for(int a = 0; a < btn.Length; a++){
            if (minDistance == distance[a]){
                minButtonNum = a;
            }
        }

        if (!dragging) {
            //LerpToBtn(minButtonNum * -btnDistance);

            LerpToBtn(-btn[minButtonNum].GetComponent<RectTransform>().anchoredPosition.x);
        }
    }

    void LerpToBtn(float position) {
		RectTransform panel = GetComponentInParent<RectTransform> ();
        float newX = Mathf.Lerp(panel.anchoredPosition.x, position, Time.deltaTime * 2.5f);
        Vector2 newPosition = new Vector2(newX, panel.anchoredPosition.y);

        panel.anchoredPosition = newPosition;
    }

    public void StartDrag(){
        dragging = true;
    }

    public void EndDrag(){
        dragging = false;
    }

	private void updateButtonDistance() {
		Button[] btn = GetComponentsInChildren<Button> ();

		for(int i = 0; i < btn.Length; i++){
			Button button = btn [i];
			Vector2 positionOffset = new Vector2 (i * btnDistance, 0);
			Vector2 newPosition = center.GetComponent<RectTransform> ().anchoredPosition + positionOffset;
			button.GetComponent<RectTransform> ().anchoredPosition = newPosition;
		}
	}
}
