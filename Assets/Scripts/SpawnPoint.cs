using UnityEngine;

public class SpawnPoint : Landmark
{
    public GameObject prefab = Resources.Load<GameObject>("Prefabs/Spawn Tile");
    public override void Generate(Transform root, Vector2Int coord)
    {
        Vector3 worldPos = WorldMap.instance.GetPosFor(coord);

        GameObject instantiated = GameObject.Instantiate(prefab, root);
        instantiated.transform.position = worldPos;
    }

    public override Vector3 AddHeight(Vector3 pos)
    {
        Debug.Log("Tried to raycast");
        if (Physics.Raycast(pos + Vector3.up * 100, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Default"))) {
            Debug.Log("Hit");
            Debug.Log(hit.transform.gameObject);
            return new Vector3(
                pos.x,
                hit.point.y,
                pos.z
            );
        } else {
            return pos;
        }

    }
}