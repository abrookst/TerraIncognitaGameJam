using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public int index;
    public DialogueSequence sequence;
    public RawImage image;
    public TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = sequence.items[index].text;
        image.texture = sequence.items[index].image;
    }

    public void Advance()
    {
        ++index;
        
        if (index == sequence.items.Count) {
            Destroy(gameObject);
        }
    }
}
