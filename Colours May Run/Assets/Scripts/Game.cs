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
    public GameObject playerPrefab;
    public GameObject pauseMenuUI;
    public GameObject highScoreUI;
    GameObject target;
    GameObject player;
    public Text clockText;
    public Text levelText;
    public Text newTime;
    public Text bestTime;

    // Control variables
    float playTime = 0f;
    float newTimeValue = 0f;
    float bestTimeValue = 0f;
    int levelNumber = 1;
    bool levelComplete = false;
    public static bool gameIsPaused = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = this.gameObject.transform.GetChild(0).gameObject;
        LoadLevel(1);
    }

    private void Update()
    {
        // Update the clock
        playTime += Time.deltaTime;
        clockText.text = playTime.ToString("0.0");

        // Spin the target
        target.transform.Rotate(Vector3.back, 1);

        // ESC key pause / unpause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused && !levelComplete)
            {
                Resume();
            } else {
                Pause();
            }
        }

        // ENTER / RETURN for next level
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadLevel(levelNumber + 1);
        }

        // BACKSPACE for reset / replay level
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            LoadLevel(levelNumber);
        }

    }


    void LoadLevel(int level)
    {
        // Set Level
        levelNumber = level;
        levelText.text = "LEVEL " + levelNumber.ToString();

        // Manage Clock
        playTime = 0f;
        gameIsPaused = false;
        Time.timeScale = 1f;
        bestTimeValue = PlayerPrefs.GetFloat("BestTime");
        levelComplete = false;

        // Toggle UI
        pauseMenuUI.SetActive(false);
        highScoreUI.SetActive(false);

        // Setup Level
        Destroy(player);
        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

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

    public void Complete()
    {
        highScoreUI.SetActive(true);
        gameIsPaused = true;
        Time.timeScale = 0f;
        if (!levelComplete)
        {
            levelComplete = true;
            newTimeValue = playTime;
            newTime.text = "YOUR TIME: " + newTimeValue.ToString("0.0");
            if (newTimeValue < bestTimeValue || bestTimeValue == 0)
            {
                bestTime.text = "NEW RECORD!";
                PlayerPrefs.SetFloat("BestTime", newTimeValue);
            } else {
                bestTime.text = "RECORD: " + bestTimeValue.ToString("0.0");
            }
        }
    }

}
