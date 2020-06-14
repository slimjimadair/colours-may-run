using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    // Define Colours
    public static Color32 white = new Color32(235, 229, 197, 255);
    public static Color32 gray = new Color32(48, 56, 58, 255);
    public static Color32 red = new Color32(209, 90, 93, 255);
    public static Color32 blue = new Color32(3, 192, 166, 255);

    // Objects
    public GameObject pauseMenuUI;
    public GameObject highScoreUI;
    public Text timeClock;
    public Text newTime;
    public Text bestTime;

    // Control variables
    float playTime = 0f;
    public static bool gameIsPaused = false;

    private void Update()
    {
        // Update the clock
        playTime = Time.time;
        timeClock.text = playTime.ToString("0.0");

        // ESC key pause / unpause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            } else {
                Pause();
            }
        }

        // ENTER for next level

        // BACKSPACE for reset / replay level



    }


    void LoadLevel(int level)
    {

    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Complete()
    {

    }

}
