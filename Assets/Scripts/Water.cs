using UnityEngine;
using System.Collections.Generic;
public class Water : Tile {
    private readonly GameObject prefab = Resources.Load<GameObject>("Prefabs/Water Tile");

    public Water(IEnumerable<Vector2Int> coords) : base(coords) {

    }

    public override void Generate() {
        foreach (Vector2Int pos in coordinates) {
            GameObject tile = GameObject.Instantiate(prefab);
            Vector3 position = 10 * pos.XYZ();
            Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90f, 0);
            tile.transform.SetPositionAndRotation(position, rotation);
            features[pos].Add(tile);
        }
    }
}