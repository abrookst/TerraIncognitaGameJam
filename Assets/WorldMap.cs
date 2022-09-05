using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum LandmarkKind {
    SpawnPoint
}

// The god-object! This thing stores all of the map data.
// It's accessed through a static singleton. Everyone uses it.
public class WorldMap : MonoBehaviour
{
    public bool initialized = false;
    public MapConfig config;
    public static WorldMap instance;

    // Important note: Vector2Int is used to address *tiles*.
    // I call these values "coordinates"
    // Vector3s are used for world-space positions.
    public readonly Dictionary<Vector2Int, Tile> map = new();
    public readonly Dictionary<Vector2Int, Landmark> landmarkMap = new();

    // Used for grading your work.
    public readonly Dictionary<TileAttribute, Dictionary<Vector2Int, float>> attributes = new();
    public readonly Dictionary<TileType, MapMarkingType> markingTypes = new() {
        {TileType.Forest, MapMarkingType.Grassy},
        {TileType.Plains, MapMarkingType.Grassy},
        {TileType.Mountains, MapMarkingType.Rocky},
        {TileType.Water, MapMarkingType.Watery},
        {TileType.Desert, MapMarkingType.Sandy},
    };
    public readonly List<Tile> tiles = new();
    public GameObject tilePrefab;
    public TileData plains;
    public TileData mountains;
    public Vector2Int bounds = new(10, 10);
    public int seed;

    public int tileSize = 10;
    void Awake()
    {
        seed = (int) Math.Floor((System.DateTime.Now - System.DateTime.UnixEpoch).TotalSeconds);
        WorldMap.instance = this;
        bounds = config.bounds;
    }

    void Start()
    {
        // I think this scales it wrongly
        Transform water = transform.Find("Water");
        water.localScale = tileSize * bounds.XYZ() / 2 + Vector3.up;
        water.position = tileSize * bounds.XYZ() / 2 + new Vector3(0, Terrain.activeTerrain.terrainData.size.y * 0.3f, 0);
    }


    // This sets up the terrain elevations. It happens first, so that
    // the tiles can figure out where to put things.    
    public void SetTerrain()
    {
        Terrain active = Terrain.activeTerrain;
        Vector2 scaledBounds = tileSize * bounds;
        active.terrainData.size = new Vector3(scaledBounds.x, active.terrainData.size.y, scaledBounds.y);
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

    // Colors the terrain. This happens later. It consults the tiles
    // to find out what the colors ought to be.
    public void ColorTerrain()
    {
        Terrain active = Terrain.activeTerrain;

        int alphaWidth = active.terrainData.alphamapWidth;
        int alphaHeight = active.terrainData.alphamapHeight;
        int count = active.terrainData.alphamapLayers;

        float[,,] alphamapData = new float[alphaWidth, alphaHeight, count];

        for (int y = 0; y < alphaHeight; y++) {
            for (int x = 0; x < alphaWidth; x++) {
                Vector2 pos = active.transform.position.XZ() +
                    new Vector2(
                        ((float) y) / alphaHeight * active.terrainData.size.x,
                        ((float) x) / alphaWidth * active.terrainData.size.z
                    );
                MapMarkingType type = markingTypes[WorldMap.instance.GetTileKind(pos.XYZ())];

                alphamapData[x,y,(int) type] = 1;
            }
        }
        float[,,] blurredData = new float[alphaWidth, alphaHeight, count];

        // This was not working right, so I disabled it.
        int blurSize = 0;
        for (int y = blurSize; y < alphaHeight-blurSize; y++) {
            for (int x = blurSize; x < alphaWidth-blurSize; x++) {
                for (int layer = 0; layer < count; layer++) {
                    for (int dx = -blurSize; dx <= blurSize; dx++) {
                        for (int dy = blurSize; dy <= blurSize; dy++) {
                            blurredData[x+dx, y+dy, layer] += alphamapData[x,y,layer] / Mathf.Pow(blurSize * 2 + 1, 2);
                        }
                    }
                }
            }
        }

        active.terrainData.SetAlphamaps(0, 0, blurredData);
    }

    public Vector3 AddTerrainHeight(Vector3 worldPos) {
        Vector2Int coord = GetCoordFor(worldPos.XZ());
        if (map.ContainsKey(coord)) {
            Vector3 tilePos = map[GetCoordFor(worldPos.XZ())].AddHeight(worldPos);
            Vector3 landmarkPos = Vector3.zero;
            if (landmarkMap.ContainsKey(coord)) {
                landmarkPos = landmarkMap[coord].AddHeight(worldPos);
            }
            return new Vector3(tilePos.x, Mathf.Max(tilePos.y, landmarkPos.y), tilePos.z);
        }
        else
            return new Vector3(worldPos.x, Terrain.activeTerrain.SampleHeight(worldPos), worldPos.z);
    }

    public void SpawnTiles()
    {
        foreach (Tile tile in tiles) {
            tile.Generate(transform);
        }
    }

    public void SpawnLandmarks()
    {
        foreach (Vector2Int pos in landmarkMap.Keys) {
            Landmark landmark = landmarkMap[pos];
            landmark.Generate(transform, pos);
        }
    }
    
    // The MapController won't try to use our data until this is set.
    public void MarkInitialized()
    {
        initialized = true;
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

    public TileType GetTileKind(Vector3 pos) {
        TileType kind = TileType.Plains;
        float elevation = config.HeightAt(pos.XZ());
        float moisture = config.MoistureAt(pos.XZ());
        float temperature = config.TemperatureAt(pos.XZ());

        if (elevation > 0.8f) {
            kind = TileType.Mountains;
        }

        if (elevation < 0.3f) {
            kind = TileType.Water;
        }

        if (moisture > 0.5f && kind == TileType.Plains) {
            kind = TileType.Forest;
        }

        if (moisture < 0.4f && kind == TileType.Plains && temperature > 0.6f)
            kind = TileType.Desert;

        return kind;
    }
}

