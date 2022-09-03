using UnityEngine;
using System.Collections.Generic;

public abstract class Tile {
    public List<Vector2Int> coordinates = new();
    public Dictionary<Vector2Int, GameObject> tiles = new();

    public abstract void Generate();

    public Tile(IEnumerable<Vector2Int> coordinates) {
        this.coordinates = new List<Vector2Int>(coordinates);
    }
}