using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    public TileData tileData;

    void Start()
    {
        Instantiate(tileData.prefab, transform);
    }
}
