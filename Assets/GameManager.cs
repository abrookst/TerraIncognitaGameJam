using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject sun;
    public MapController map;

    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;

        // FIXME this is bad
        sun = GameObject.FindObjectOfType<Light>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float t = (Time.time - startTime) / WorldMap.instance.config.timeLimit;
        sun.transform.rotation = Quaternion.Euler(Mathf.Lerp(0, 180, t), 90, 0);

        if (t >= 1) {
            OfficeController.lastScore = WorldMap.instance.ScoreMap(map);
            FinishLevel();
        }
    }

    public void FinishLevel()
    {

        SceneManager.LoadScene("OfficeScene");
    }
}
