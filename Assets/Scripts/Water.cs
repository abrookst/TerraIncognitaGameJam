using UnityEngine;
using System.Collections.Generic;
public class Water : Tile {
    private readonly GameObject prefab = Resources.Load<GameObject>("Prefabs/Water Tile");

    public Water(IEnumerable<Vector2Int> coords) : base(coords) {
        marking = MapMarkingType.Watery;
    }

    public override void Generate(Transform root) {
        GameObject holderObj = new();
        Transform holder = holderObj.transform;
        holder.SetParent(root);
        holder.position = Vector3.zero;
        foreach (Vector2Int pos in coordinates) {
            GameObject tile = GameObject.Instantiate(prefab, holder);
            Vector3 position = WorldMap.instance.GetPosFor(pos);
            position.y = Terrain.activeTerrain.terrainData.size.y * 0.3f;
            Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90f, 0);
            tile.transform.SetPositionAndRotation(position, rotation);
            features[pos].Add(tile);
        }
    }
}