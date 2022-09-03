using UnityEngine;
using System.Collections.Generic;
public class Mountain : Tile {
    private readonly GameObject prefab = Resources.Load<GameObject>("Prefabs/Mountain Tile");

    public Mountain(IEnumerable<Vector2Int> coords) : base(coords) {
        
    }

    public override void Generate() {
        foreach (Vector2Int pos in coordinates) {
            GameObject tile = GameObject.Instantiate(prefab);
            tiles[pos] = tile;
            Vector3 position = 10 * pos.XYZ();
            Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90f, 0);
            tiles[pos].transform.SetPositionAndRotation(position, rotation);

            if (coordinates.Contains(pos + Vector2Int.down)) {
                tile = GameObject.Instantiate(prefab);
                position = 10 * pos.XYZ() + 5 * Vector2Int.down.XYZ();
                rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90f, 0);
                tile.transform.SetPositionAndRotation(position, rotation);
            }

            if (coordinates.Contains(pos + Vector2Int.right)) {
                tile = GameObject.Instantiate(prefab);
                position = 10 * pos.XYZ() + 5 * Vector2Int.right.XYZ();
                rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90f, 0);
                tile.transform.SetPositionAndRotation(position, rotation);
            }
        }
    }
}