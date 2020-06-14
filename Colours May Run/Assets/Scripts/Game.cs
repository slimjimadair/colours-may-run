using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SocialPlatforms;
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
    GameObject platformContainer;
    GameObject shadowContainer;
    public Text clockText;
    public Text levelText;
    public Text newTime;
    public Text bestTime;

    // Level Handling
    string levelPath;
    string levelJsonStr;
    LevelData levelData;
    int totalLevels;
    public GameObject grayPrefab;
    public GameObject redPrefab;
    public GameObject bluePrefab;
    public GameObject shadowPrefab;

    // Control variables
    float playTime = 0f;
    float newTimeValue = 0f;
    float bestTimeValue = 0f;
    int levelNumber = 1;
    bool levelComplete = false;
    public static bool gameIsPaused = false;

    private void Start()
    {
        // Get level settings
        levelPath = Application.dataPath + "/Data/levels.json";
        levelJsonStr = File.ReadAllText(levelPath);
        levelData = JsonUtility.FromJson<LevelData>(levelJsonStr);
        totalLevels = levelData.levels.Length;

        // Set up objects
        player = GameObject.FindGameObjectWithTag("Player");
        target = this.gameObject.transform.GetChild(0).gameObject;
        platformContainer = GameObject.Find("Platforms");
        shadowContainer = GameObject.Find("Shadows");

        // Load start level
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
            Restart();
        }

    }


    void LoadLevel(int level)
    {
        // Set Level
        levelNumber = level;
        if (levelNumber > totalLevels || levelNumber <= 0)
        {
            levelNumber = 1;
        }
        levelText.text = "LEVEL " + levelNumber.ToString();

        // Manage Clock
        playTime = 0f;
        gameIsPaused = false;
        Time.timeScale = 1f;
        string bestTimePref = "BestTime" + level.ToString("000");
        bestTimeValue = PlayerPrefs.GetFloat(bestTimePref);
        levelComplete = false;

        // Toggle UI
        pauseMenuUI.SetActive(false);
        highScoreUI.SetActive(false);

        // Clear Old Level
        Destroy(player);
        foreach (Transform platform in platformContainer.transform)
        {
            GameObject.Destroy(platform.gameObject);
        }
        foreach (Transform shadow in shadowContainer.transform)
        {
            GameObject.Destroy(shadow.gameObject);
        }

        // Setup New Level
        foreach (Platform platform in levelData.levels[levelNumber - 1].platforms)
        {
            GameObject platformPrefab;
            switch(platform.c)
            {
                case "Gray":
                    platformPrefab = grayPrefab;
                    break;

                case "Red":
                    platformPrefab = redPrefab;
                    break;

                case "Blue":
                    platformPrefab = bluePrefab;
                    break;

                default:
                    platformPrefab = grayPrefab;
                    break;
            }
            GameObject platformObject = Instantiate(platformPrefab, new Vector3(platform.x, platform.y, 0), Quaternion.identity);
            platformObject.tag = platform.c;
            platformObject.transform.SetParent(platformContainer.transform);
            platformObject.transform.localScale = new Vector3(platform.w, platform.h, 1);

            GameObject shadowObject = Instantiate(shadowPrefab, new Vector3(platform.x, platform.y, -1), Quaternion.identity);
            shadowObject.transform.SetParent(shadowContainer.transform);
            float angle = 3 * ((Mathf.RoundToInt(Random.value) * 2) - 1);
            shadowObject.transform.Rotate(Vector3.back, angle);
            float scaleModifier = Random.Range(1f, 1.1f);
            shadowObject.transform.localScale = new Vector3(platform.w * scaleModifier, platform.h * scaleModifier, 1);
        }
        Target targetSetting = levelData.levels[levelNumber - 1].target;
        target.transform.position = new Vector3(targetSetting.x, targetSetting.y, 0);
        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

    }

    public void Restart()
    {
        LoadLevel(levelNumber);
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
                string bestTimePref = "BestTime" + levelNumber.ToString("000");
                PlayerPrefs.SetFloat(bestTimePref, newTimeValue);
            } else {
                bestTime.text = "RECORD: " + bestTimeValue.ToString("0.0");
            }
        }
    }

    [System.Serializable]
    public class LevelData
    {
        public Level[] levels;
    }

    [System.Serializable]
    public class Level
    {
        public int levelID;
        public Target target;
        public Platform[] platforms;
    }

    [System.Serializable]
    public class Platform
    {
        public float x;
        public float y;
        public float h;
        public float w;
        public string c;
    }

    [System.Serializable]
    public class Target
    {
        public float x;
        public float y;
    }

}
