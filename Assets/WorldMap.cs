using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WorldMap : MonoBehaviour
{
    public bool initialized = false;
    public MapConfig config;
    public static WorldMap instance;
    public readonly Dictionary<Vector2Int, Tile> map = new();
    public readonly Dictionary<TileAttribute, Dictionary<Vector2Int, float>> attributes = new();
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

    public void SetTerrain()
    {
        Terrain active = Terrain.activeTerrain;
        int resolution = active.terrainData.heightmapResolution;
        float[,] heightData= new float[resolution, resolution];

        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++) {
                float remapFactor = active.terrainData.size.x / resolution;
                Vector2 coord = new(x * remapFactor, y * remapFactor);
                float height = config.HeightAt(coord);
                heightData[y,x] = height;
            }
        }
        active.terrainData.SetHeights(0, 0, heightData);
    }

    public Vector3 AddTerrainHeight(Vector3 worldPos) {
        Vector3 result = worldPos;
        result = result + new Vector3(0, Terrain.activeTerrain.SampleHeight(worldPos), 0);
        return result;
    }

    public void SpawnTiles()
    {
        foreach (Tile tile in tiles) {
            tile.Generate(transform);
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

    public Vector3 GetPosFor(Vector2Int coord)
    {  
        Vector3 result = new(
            coord.x * tileSize + tileSize / 2.0f,
            0,
            coord.y * tileSize + tileSize / 2.0f
        );

        return AddTerrainHeight(result);
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
                Gizmos.DrawSphere(GetPosFor(coord), 1f);
            }
        }
    }

    public void FillMap(MapController mapController) {
        foreach (Vector2Int pos in map.Keys) {
            mapController.markings[pos] = map[pos].marking;
        }

        mapController.Refresh();
    }

    public float ScoreMap(MapController mapController) {
        float points = 0;
        foreach (Vector2Int pos in map.Keys) {
            if (mapController.markings[pos] == map[pos].marking) {
                points += 1;
            }
        }

        return points / map.Count;
    }
}

