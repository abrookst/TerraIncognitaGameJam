using UnityEngine;
using System.Collections.Generic;
public class Plains : Tile {
    private readonly GameObject prefab = Resources.Load<GameObject>("Prefabs/Plains Tile");

    public Plains(IEnumerable<Vector2Int> coords) : base(coords) {
        
    }

    public override void Generate() {
        foreach (Vector2Int pos in coordinates) {
            GameObject tile = GameObject.Instantiate(prefab);
            tiles[pos] = tile;
            Vector3 position = 10 * pos.XYZ();
            Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90f, 0);
            tiles[pos].transform.SetPositionAndRotation(position, rotation);
        }
    }
}