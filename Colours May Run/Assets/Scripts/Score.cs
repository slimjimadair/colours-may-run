using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    float playTime = 0f;
    public Text timeText;

    void Update()
    {
        playTime = Time.time;
        timeText.text = playTime.ToString("0.0");
    }
}
