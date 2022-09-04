using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Game Progress Data", menuName = "Create Progress Data")]
public class GameProgress : ScriptableObject, ISerializationCallbackReceiver
{
    [NonSerialized]
    public float lastScore;

    [NonSerialized]
    public int level = 0;

    public int startLevel = 0;

    public void OnAfterDeserialize()
    {
        level = startLevel;
    }

    public void OnBeforeSerialize() { }
}
