using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class scrollRect_CS : MonoBehaviour {
    public RectTransform panel; //ScroolPanel

    public Button[] btn;
    public RectTransform center;

    public float[] distance;
    public float[] distanceRepo;
    private bool dragging = false; //true = drag
    private int btnDistance;
    private int minButtonNum;
    private int btnLenght;

    void Start(){
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Music_menu/");
        FileInfo[] fichiers = dir.GetFiles();

        List<String> list = new List<String>();

        foreach (FileInfo fichier in fichiers)
        {

            list.Add(fichier.Name.ToString());
        }

        btnLenght = btn.Length;
        distance = new float[btnLenght];
        distanceRepo = new float[btnLenght];

        btnDistance = (int)Mathf.Abs(btn[1].GetComponent<RectTransform>().anchoredPosition.x - btn[0].GetComponent<RectTransform>().anchoredPosition.x);
  
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
