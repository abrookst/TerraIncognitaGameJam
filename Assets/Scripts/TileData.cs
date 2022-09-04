using UnityEngine;

public enum TileType
{
    Plains,
    Mountains,
    Forest,
    Water,
    Desert
}

[CreateAssetMenu(fileName = "Tile", menuName = "Tile Data")]
public class TileData : ScriptableObject
{
    public TileType kind;
    public GameObject prefab;
}