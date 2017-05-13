using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectSnap_CS : MonoBehaviour {

    //Public Variables
    public RectTransform panel; // To hold the ScrollPanel
    public Button[] bttn;
    public RectTransform center; // Center to compare the distance for each button

    // Private Variables
    public float[] distance;
    private bool dragging = false;
    private int bttnDistance; // Will hold the distance between the buttons
    private int minButtonNum; // To hold the number of the button, with smallest distance to center

    private void Start()
    {
        int bttnLenght = bttn.Length;
        distance = new float[bttnLenght];

        bttnDistance = (int)Mathf.Abs(bttn[1].GetComponent<RectTransform>().anchoredPosition.x - bttn[0].GetComponent<RectTransform>().anchoredPosition.x);
    }

    private void Update()
    {
        for (int i = 0; i < bttn.Length; i++)
        {
            distance[i] = Mathf.Abs(center.transform.position.x - bttn[i].transform.position.x);

        }

        float minDistance = Mathf.Min(distance); // Get the min distance

        for (int i = 0; i < bttn.Length; i++)
        {
            if (minDistance == distance[i])
            {
                minButtonNum = i;
            }
        }

        if (!dragging)
        {
            LerpToBttn(minButtonNum * -bttnDistance);

        }

    }

    void LerpToBttn(int position)
    {
        float newx = Mathf.Lerp(panel.anchoredPosition.x, position, Time.deltaTime * 3f);
        Vector2 newPosition = new Vector2(newx, panel.anchoredPosition.y);

        panel.anchoredPosition = newPosition;
    }

    public void StartDrag()
    {
        dragging = true;
    }

    public void EndDrag()
    {
        dragging = false;
    }


}
