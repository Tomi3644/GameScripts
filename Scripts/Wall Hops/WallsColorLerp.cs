using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class WallsColorLerp : MonoBehaviour
{
    public List<Color> colors;
    public Material mat;
    Color targetColor;
    int colorID;

    private void Awake()
    {
        colorID = 0;
        targetColor = colors[colorID];
    }

    public void ChangeID()
    {
        if (colorID == 10)
        {
            colorID = 0;
            targetColor = colors[colorID];
        }
        else
        {
            colorID += 1;
            targetColor = colors[colorID];
        }
    }
    private void Update()
    {
        //mat.SetColor("_EmissionColor", colors[colorID]);
        mat.SetColor("_EmissionColor", Color.Lerp(mat.GetColor("_EmissionColor"), targetColor, 0.01f));

    }
}

