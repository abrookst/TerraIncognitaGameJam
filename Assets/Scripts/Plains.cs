using UnityEngine;
using System.Collections.Generic;
public class Plains : Tile {
    private readonly GameObject prefab = Resources.Load<GameObject>("Prefabs/Plains Tile");

    public Plains(IEnumerable<Vector2Int> coords) : base(coords) {
        marking = MapMarkingType.Grassy;
    }

    public override void Generate(Transform root) {
        GameObject holderObj = new();
        Transform holder = holderObj.transform;
        holder.SetParent(root);
        holder.position = Vector3.zero;
        foreach (Vector2Int pos in coordinates) {
            GameObject tile = GameObject.Instantiate(prefab, holder);
            Vector3 position = WorldMap.instance.GetPosFor(pos);
            Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90f, 0);
            tile.transform.SetPositionAndRotation(position, rotation);
            features[pos].Add(tile);
        }
    }
}