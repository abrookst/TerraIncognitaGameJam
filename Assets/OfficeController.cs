using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum OfficeStates {
    Computer,
    Pinboard
}
public class OfficeController : MonoBehaviour
{
    public static bool gameStarted = false;
    public static int level = 0;
    public static float lastScore = -1;
    public Transform buttonHolder;
    public TMP_Text scoreText;
    public Dialogue dialogue;
    public List<DialogueSequence> dialogueSequences = new();
    public OfficeStates state;
    public CinemachineVirtualCamera computerCam;
    public CinemachineVirtualCamera pinboardCam;
    public GameObject settingsMenu;
    public MapConfig mapSettings;

    public List<MapConfig> configs = new();

    void Awake()
    {
        if (!gameStarted) {
            gameStarted = true;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Setting dialogue for level " + level);
        dialogue.sequence = dialogueSequences[level];
        for (int i = 0; i < buttonHolder.childCount; i++) {
            buttonHolder.GetChild(i).gameObject.SetActive(level >= i);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        computerCam.enabled = state == OfficeStates.Computer;
        pinboardCam.enabled = state == OfficeStates.Pinboard;

        if (lastScore >= 0) {
            scoreText.text = "Score: \n" + Mathf.Round(lastScore * 100) + "%";
        }
    }

    public void StartGame()
    {
        if (settingsMenu.transform.Find("Size").Find("Small").GetComponent<Toggle>().isOn) {
            mapSettings.bounds = new Vector2Int(5, 5);
        } else if (settingsMenu.transform.Find("Size").Find("Medium").GetComponent<Toggle>().isOn) {
            mapSettings.bounds = new Vector2Int(10, 10);
        } else if (settingsMenu.transform.Find("Size").Find("Large").GetComponent<Toggle>().isOn) {
            mapSettings.bounds = new Vector2Int(20, 20);
        }

        if (settingsMenu.transform.Find("Climate").Find("Cold").GetComponent<Toggle>().isOn) {
            mapSettings.minTemperature = 0;
            mapSettings.maxTemperature = 0.6f;
        } else if (settingsMenu.transform.Find("Climate").Find("Mild").GetComponent<Toggle>().isOn) {
            mapSettings.minTemperature = 0.2f;
            mapSettings.maxTemperature = 0.8f;
        } else if (settingsMenu.transform.Find("Climate").Find("Hot").GetComponent<Toggle>().isOn) {
            mapSettings.minTemperature = 0.5f;
            mapSettings.maxTemperature = 1f;
        }
        if (settingsMenu.transform.Find("Terrain").Find("Lakes").GetComponent<Toggle>().isOn) {
            mapSettings.minElevation = 0.1f;
            mapSettings.maxElevation = 0.6f;
        } else if (settingsMenu.transform.Find("Terrain").Find("Flat").GetComponent<Toggle>().isOn) {
            mapSettings.minElevation = 0.4f;
            mapSettings.maxElevation = 0.7f;
        } else if (settingsMenu.transform.Find("Terrain").Find("Hilly").GetComponent<Toggle>().isOn) {
            mapSettings.minElevation = 0.5f;
            mapSettings.maxElevation = 1f;
        }

        SceneManager.LoadScene("GameScene");
    }

    public void GoToComputer()
    {
        state = OfficeStates.Computer;
    }

    public void GoToPinboard()
    {
        state = OfficeStates.Pinboard;
    }

    public void StartLevel(int index) {
        level = index;
        Debug.Log(level);
        MapConfig chosen = configs[index - 1];
        mapSettings.timeLimit = chosen.timeLimit;
        mapSettings.bounds = chosen.bounds;
        mapSettings.minTemperature = chosen.minTemperature;
        mapSettings.maxTemperature = chosen.maxTemperature;
        mapSettings.minElevation = chosen.minElevation;
        mapSettings.minElevation = chosen.minElevation;
        mapSettings.minMoisture = chosen.minMoisture;
        mapSettings.minMoisture = chosen.minMoisture;
        mapSettings.elevationCurve = chosen.elevationCurve;

        SceneManager.LoadScene("GameScene");
    }
}
