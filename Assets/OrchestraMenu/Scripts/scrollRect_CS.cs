using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class scrollRect_CS : MonoBehaviour {
    public RectTransform center;
	public GameObject btnPrefab;
	[RangeAttribute(0, 1000)] public float btnDistance = 300;
	private RectTransform panel;

    public float[] distance;
    public float[] distanceRepo;
    private bool dragging = false; //true = drag
	private Button[] btn;
    private int minButtonNum;
    private int btnLenght;

	void Awake(){
		DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Music_menu/");
		FileInfo[] fichiers = dir.GetFiles();

		List<String> list = new List<String>();

		foreach (FileInfo fichier in fichiers) {
			if (fichier.Extension != ".mid") continue;

			GameObject btnObject = Instantiate(btnPrefab) as GameObject;
			btnObject.transform.SetParent(transform);
			btnObject.GetComponent<MenuButton>().midiPath = fichier;
		}

		btn = GetComponentsInChildren<Button> ();
		panel = GetComponentInParent<RectTransform> ();
	}

    void Start(){
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Music_menu/");
        FileInfo[] fichiers = dir.GetFiles();

		for(int i = 0; i < btn.Length; i++){
			Button button = btn [i];
			Vector2 positionOffset = new Vector2 (i * btnDistance, 0);
			Vector2 newPosition = center.GetComponent<RectTransform> ().anchoredPosition + positionOffset;
			button.GetComponent<RectTransform> ().anchoredPosition = newPosition;
		}

        btnLenght = btn.Length;
        distance = new float[btnLenght];
        distanceRepo = new float[btnLenght];
    }

    void Update(){
        for (int i = 0; i < btn.Length; i++) {
            distanceRepo[i] = center.GetComponent<RectTransform>().position.x - btn[i].GetComponent<RectTransform>().position.x;
            distance[i] = Mathf.Abs(distanceRepo[i]);

            if (distanceRepo[i] > 1000){
                float curX = btn[i].GetComponent<RectTransform>().anchoredPosition.x;
                float curY = btn[i].GetComponent<RectTransform>().anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX + (btnLenght * btnDistance), curY);
                btn[i].GetComponent<RectTransform>().anchoredPosition = newAnchoredPos;
            }

            if (distanceRepo[i] < -1000)
            {
                float curX = btn[i].GetComponent<RectTransform>().anchoredPosition.x;
                float curY = btn[i].GetComponent<RectTransform>().anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX - (btnLenght * btnDistance), curY);
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
}
