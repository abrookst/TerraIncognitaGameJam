using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Game Progress Data", menuName = "Create Progress Data")]
public class GameProgress : ScriptableObject
{
    public float lastScore;
    [NonSerialized]
    public bool initialized;

    public int level = 0;

    public int startLevel = 0;

    public void Initialize()
    {
        Debug.Log("Initializing");
        initialized = true;
        level = startLevel;
        lastScore = -1;
    }
}
