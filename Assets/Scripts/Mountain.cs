using UnityEngine;
using System.Collections.Generic;
public class Mountain : Tile {
    private readonly GameObject prefab = Resources.Load<GameObject>("Prefabs/Mountain Tile");

    public Mountain(IEnumerable<Vector2Int> coords) : base(coords) {
        
    }

    public override void Generate() {
        foreach (Vector2Int pos in coordinates) {
            GameObject tile = GameObject.Instantiate(prefab);
            Vector3 position = 10 * pos.XYZ();
            Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360f), 0);
            features[pos].Add(tile);
            Vector3 posOffset = UnityEngine.Random.onUnitSphere;
            posOffset.y = 0;
            posOffset *= 2;
            position += posOffset;
            tile.transform.SetPositionAndRotation(position, rotation);
            tile.transform.localScale += 0.1f * UnityEngine.Random.onUnitSphere;

            if (coordinates.Contains(pos + Vector2Int.down)) {
                tile = GameObject.Instantiate(prefab);
                position = 10 * pos.XYZ() + 5 * Vector2Int.down.XYZ();
                posOffset = UnityEngine.Random.onUnitSphere;
                posOffset.y = 0;
                posOffset *= 2;
                position += posOffset;
                rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360f), 0);
                tile.transform.SetPositionAndRotation(position, rotation);
                tile.transform.localScale += 0.1f * UnityEngine.Random.onUnitSphere;
                features[pos].Add(tile);
            }

            if (coordinates.Contains(pos + Vector2Int.right)) {
                tile = GameObject.Instantiate(prefab);
                position = 10 * pos.XYZ() + 5 * Vector2Int.right.XYZ();
                posOffset = UnityEngine.Random.onUnitSphere;
                posOffset.y = 0;
                posOffset *= 2;
                position += posOffset;
                rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360f), 0);
                tile.transform.localScale += 0.1f * UnityEngine.Random.onUnitSphere;
                tile.transform.SetPositionAndRotation(position, rotation);
                features[pos].Add(tile);
            }
        }
    }
}