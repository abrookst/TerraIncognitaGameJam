using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WorldMap : MonoBehaviour
{
    public readonly Dictionary<Vector2Int, Tile> map = new();
    public readonly List<Tile> tiles = new();
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
            Vector2Int secondCorner = firstCorner + new Vector2Int(3, 3).Random();

            Debug.Log(firstCorner);
            Debug.Log(secondCorner);

            List<Vector2Int> coords = VectorUtils.Area(firstCorner, secondCorner).ToList();

            // Drop some of the tiles
            coords = coords.Where(x => UnityEngine.Random.Range(0f, 1f) < 0.6f).ToList();

            if (coords.All(pos => IsLegalPosition(pos)))
            {
                Mountain mountain = new(coords);
                foreach (Vector2Int pos in coords)
                {
                    map[pos] = mountain;
                }
                
                tiles.Add(mountain);
            }

        }
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector2Int coord = new(x, y);

                if (IsLegalPosition(coord))
                {
                    map[coord] = new Plains(new Vector2Int[] { coord });
                    tiles.Add(map[coord]);
                }
            }
        }

        SpawnTiles();

    }

    void SpawnTiles()
    {
        foreach (Tile tile in tiles) {
            tile.Generate();
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

