using UnityEngine;
using System.Collections.Generic;
public class Water : Tile {
    private readonly GameObject prefab = Resources.Load<GameObject>("Prefabs/Water Tile");
    public Dictionary<Vector2Int, bool> frozen = new();

    public override bool Passable(Vector2Int pos) {
        return frozen[pos];
    }

    public override Vector3 AddHeight(Vector3 worldPos) {
        Vector2Int pos = WorldMap.instance.GetCoordFor(worldPos.XZ());
        if (frozen.ContainsKey(pos) && frozen[pos])
            return new Vector3(worldPos.x, Mathf.Max(0.3f * Terrain.activeTerrain.terrainData.size.y, Terrain.activeTerrain.SampleHeight(worldPos)), worldPos.z);
        else
            return new Vector3(worldPos.x, Terrain.activeTerrain.SampleHeight(worldPos), worldPos.z);
    }

    public Water(IEnumerable<Vector2Int> coords) : base(coords) {
        marking = MapMarkingType.Watery;
    }

    public override void Generate(Transform root) {
        GameObject holderObj = new();
        Transform holder = holderObj.transform;
        holder.SetParent(root);
        holder.position = Vector3.zero;
        foreach (Vector2Int pos in coordinates) {
            frozen[pos] = WorldMap.instance.config.TemperatureAt(WorldMap.instance.GetPosFor(pos)) < 0.4f;
            if (!frozen[pos])
                continue;
            GameObject tile = GameObject.Instantiate(prefab, holder);
            Vector3 position = WorldMap.instance.GetPosFor(pos);
            position.y = Terrain.activeTerrain.terrainData.size.y * 0.3f;
            Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90f, 0);
            tile.transform.SetPositionAndRotation(position, rotation);
            features[pos].Add(tile);
        }
    }
}