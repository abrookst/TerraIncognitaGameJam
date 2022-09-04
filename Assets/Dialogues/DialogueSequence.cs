using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct DialogueItem {
    public string text;
    public Texture image;
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue Sequence")]
public class DialogueSequence : ScriptableObject
{
    public List<DialogueItem> items;
}
