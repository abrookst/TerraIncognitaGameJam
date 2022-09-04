using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Forest : Tile {
    private readonly GameObject prefab = Resources.Load<GameObject>("Prefabs/Forest Tile");
    private readonly GameObject tree = Resources.Load<GameObject>("Prefabs/Forest Tree");

    public Forest(IEnumerable<Vector2Int> coords) : base(coords) {
        marking = MapMarkingType.Grassy;
    }

    public override void Generate(Transform root) {
        GameObject holderObj = new();
        Transform holder = holderObj.transform;
        holder.SetParent(root);
        holder.position = Vector3.zero;
        
        Vector2Int min = coordinates.Aggregate((vec1, vec2) => {
            return new Vector2Int(Mathf.Min(vec1.x, vec2.x), Mathf.Min(vec1.y, vec2.y));
        });
        Vector2Int max = coordinates.Aggregate((vec1, vec2) => {
            return new Vector2Int(Mathf.Max(vec1.x, vec2.x), Mathf.Max(vec1.y, vec2.y));
        });
        Vector2Int center = (min + max) / 2;

        float radius = ((max - min).magnitude + 2) * WorldMap.instance.tileSize;

        foreach (Vector2Int pos in coordinates) {
            GameObject tile = GameObject.Instantiate(prefab, holder);
            Vector3 position = WorldMap.instance.GetPosFor(pos);
            Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90f, 0);
            tile.transform.SetPositionAndRotation(position, rotation);
            features[pos].Add(tile);
        }

        int trees = UnityEngine.Random.Range(coordinates.Count * 14, coordinates.Count * 20);
        
        List<Vector3> positions = new();
        while(trees > 0) {
            --trees;
            Vector2Int coord = coordinates.Pick();
            Vector3 position = WorldMap.instance.GetPosFor(coord);
            float maxOffset = WorldMap.instance.tileSize/2;
            position += new Vector3(
                UnityEngine.Random.Range(-maxOffset, maxOffset),
                0,
                UnityEngine.Random.Range(-maxOffset, maxOffset)
            );
            Vector3 treePosition = position;

            Vector2 centerDelta = treePosition.XZ() - WorldMap.instance.GetPosFor(coord).XZ();
            if (Mathf.Abs(centerDelta.x) < 2f || Mathf.Abs(centerDelta.y) < 2f)
                continue;

            treePosition = WorldMap.instance.AddTerrainHeight(new Vector3(treePosition.x, 0, treePosition.z));

            GameObject treeObj = GameObject.Instantiate(tree, holder);
            treeObj.transform.position = treePosition;
            Vector3 newScale = treeObj.transform.localScale;
            newScale.Scale(new Vector3(1, UnityEngine.Random.Range(0.7f, 1.5f), 1));
            treeObj.transform.localScale = newScale;
        }
    }
}