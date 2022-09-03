using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WorldMap : MonoBehaviour
{
    public readonly Dictionary<Vector2Int, TileData> map = new();
    public readonly Dictionary<Vector2Int, GameObject> tiles = new();
    public GameObject tilePrefab;
    public TileData plains;
    public TileData mountains;
    public Vector2Int bounds = new(10, 10);

    void Start()
    {
        // this will move out of here later

        // put down some BIG features first

        for (int i = 0; i < 5; i++)
        {
            Vector2Int firstCorner = bounds.Random();
            Vector2Int secondCorner = firstCorner + new Vector2Int(1, 1);

            Debug.Log(firstCorner);
            Debug.Log(secondCorner);

            if (IsLegalArea(firstCorner, secondCorner))
            {
                foreach (Vector2Int pos in VectorUtils.Area(firstCorner, secondCorner))
                {
                    map[pos] = mountains;
                }

            }

        }
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector2Int coord = new(x, y);

                if (IsLegalPosition(coord))
                {
                    if (UnityEngine.Random.Range(0f, 1f) < 0.75f)
                        map[coord] = plains;
                    else
                        map[coord] = mountains;
                }
            }
        }

        SpawnTiles();

    }

    void SpawnTiles()
    {
        foreach (Vector2Int pos in map.Keys)
        {
            tiles[pos] = Instantiate(tilePrefab);
            MapTile mapTile = tiles[pos].GetComponent<MapTile>();
            mapTile.tileData = map[pos];
            Vector3 position = 10 * pos.XYZ();
            Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90f, 0);
            tiles[pos].transform.SetPositionAndRotation(position, rotation);
        }
    }

    public bool IsLegalPosition(Vector2Int pos)
    {
        return InBounds(pos) && (!map.ContainsKey(pos));
    }

    // Is this area fine to put stuff in? In bounds and clear.
    public bool IsLegalArea(Vector2Int a, Vector2Int b)
    {
        return VectorUtils.Area(a, b).All(pos => InBounds(pos)) && IsClear(a, b);
    }

    // Is this (zero-indexed) position in bounds?
    public bool InBounds(Vector2Int pos)
    {
        return !(pos.x < 0 || pos.y < 0 || pos.x >= bounds.x || pos.y >= bounds.y);
    }

    // Does anything exist in the area covered by these two Vector2Ints?
    public bool IsClear(Vector2Int a, Vector2Int b)
    {
        return !VectorUtils.Area(a, b).Any(coord => map.ContainsKey(coord));
    }
}

