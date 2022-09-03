using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WorldMap : MonoBehaviour
{
    public static WorldMap instance;
    public readonly Dictionary<Vector2Int, Tile> map = new();
    public readonly List<Tile> tiles = new();
    public GameObject tilePrefab;
    public TileData plains;
    public TileData mountains;
    public Vector2Int bounds = new(10, 10);

    public int tileSize = 10;

    void Awake()
    {
        WorldMap.instance = this;
    }

    public void SpawnTiles()
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

    public Vector2 GetPosFor(Vector2Int coord)
    {  
        return new Vector2(
            coord.x * tileSize + tileSize / 2.0f,
            coord.y * tileSize + tileSize / 2.0f
        );
    }

    public Vector2Int GetCoordFor(Vector2 pos) {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / tileSize),
            Mathf.FloorToInt(pos.y / tileSize)
        );
    }

    public void OnDrawGizmos()
    {
        foreach (Tile tile in tiles) {
            foreach (Vector2Int coord in tile.coordinates) {
                Gizmos.DrawSphere(GetPosFor(coord).XYZ(), 1f);
            }
        }
    }
}

