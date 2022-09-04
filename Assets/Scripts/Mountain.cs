using UnityEngine;
using System.Collections.Generic;
public class Mountain : Tile
{
    private readonly GameObject prefab = Resources.Load<GameObject>("Prefabs/Mountain Tile");

    public override bool Passable(Vector2Int pos)
    {
        return false;
    }
    public Mountain(IEnumerable<Vector2Int> coords) : base(coords)
    {
        marking = MapMarkingType.Rocky;
    }

    private void PutMountain(Vector2Int coord, Vector3 pos, Transform holder)
    {
        GameObject tile = GameObject.Instantiate(prefab, holder);
        Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360f), 0);
        features[coord].Add(tile);
        Vector3 posOffset = UnityEngine.Random.onUnitSphere;
        posOffset.y = 0;
        posOffset *= 2;
        pos += posOffset;
        tile.transform.SetPositionAndRotation(pos, rotation);
        tile.transform.localScale += 0.1f * UnityEngine.Random.onUnitSphere;
    }
    public override void Generate(Transform root)
    {
        GameObject holderObj = new();
        Transform holder = holderObj.transform;
        holder.SetParent(root);
        holder.position = Vector3.zero;

        foreach (Vector2Int pos in coordinates)
        {

            PutMountain(
                pos,
                WorldMap.instance.GetPosFor(pos),
                holder
            );

            if (coordinates.Contains(pos + Vector2Int.down))
            {
                PutMountain(
                    pos,
                    (WorldMap.instance.GetPosFor(pos) + WorldMap.instance.GetPosFor(pos + Vector2Int.down)) / 2,
                    holder
                );
            }

            if (coordinates.Contains(pos + Vector2Int.right))
            {
                PutMountain(
                    pos,
                    (WorldMap.instance.GetPosFor(pos) + WorldMap.instance.GetPosFor(pos + Vector2Int.right)) / 2,
                    holder
                );
            }
        }
    }
}