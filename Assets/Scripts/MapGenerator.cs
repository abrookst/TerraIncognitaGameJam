using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

public enum TileAttribute {
    Elevation,
    Moisture,
    Temperature
}

public class MapGenerator : MonoBehaviour
{
    int limit = 100;
    private readonly Dictionary<Vector2Int, TileType> remaining = new();
    public WorldMap map;


    void Start()
    {
        foreach (TileAttribute attribute in Enum.GetValues(typeof(TileType)))
        {
            map.attributes[attribute] = new();
        }

        limit = 100000;
        // FIXME: needs an exclusive version of Area
        foreach (Vector2Int coord in VectorUtils.Area(map.bounds - new Vector2Int(1, 1)))
        {
            Vector3 worldPos = WorldMap.instance.GetPosFor(coord);

            float elevation = WorldMap.instance.config.HeightAt(worldPos.XZ());
            map.attributes[TileAttribute.Elevation][coord] = elevation;
            float moisture =  WorldMap.instance.config.MoistureAt(worldPos.XZ());
            map.attributes[TileAttribute.Moisture][coord] = moisture;
            
            remaining[coord] = TileType.Plains;
            if (elevation > 0.8f) {
                remaining[coord] = TileType.Mountains;
            }

            if (elevation < 0.3f) {
                remaining[coord] = TileType.Water;
            }

            if (moisture > 0.5f && remaining[coord] == TileType.Plains) {
                remaining[coord] = TileType.Forest;
            }
        }

        // Flood-fill to group tiles up.
        while (remaining.Count > 0)
        {
            // Debug.Log(remaining.Count);
            // Debug.Log(remaining.Keys.First());
            if (--limit <= 0)
            {
                Debug.Log("I'm stuck!");
                Debug.Log(remaining.Count);
                Debug.Log(remaining.Keys.First());
                throw new System.Exception();
            }

            // bad? yeah. fast enough tho
            List<Vector2Int> positions = new(remaining.Keys);
            int index = UnityEngine.Random.Range(0, positions.Count);
            Vector2Int pos = positions[index];
            TileType kind = remaining[pos];
            var foundTiles = FloodFill(pos, kind);

            Tile tile = kind switch
            {
                TileType.Mountains => new Mountain(foundTiles),
                TileType.Plains => new Plains(foundTiles),
                TileType.Forest => new Forest(foundTiles),
                TileType.Water => new Water(foundTiles),
                _ => null
            };

            foreach (Vector2Int position in foundTiles)
            {
                map.map[position] = tile;
            }

            map.tiles.Add(tile);
        }

        map.SetTerrain();
        map.SpawnTiles();
        map.initialized = true;
    }

    List<Vector2Int> FloodFill(Vector2Int pos, TileType kind)
    {
        List<Vector2Int> pending = new() { pos };
        List<Vector2Int> result = new();


        // We do this in the inner loop so that we dont get duplicates,
        // but this means we miss the starting point!
        remaining.Remove(pos);
        while (pending.Count > 0)
        {
            Vector2Int nextPos = pending[0];
            pending.RemoveAt(0);
            result.Add(nextPos);

            foreach (Vector2Int dir in new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left })
            {
                Vector2Int neighbor = nextPos + dir;
                if (map.InBounds(neighbor) && remaining.ContainsKey(neighbor) && remaining[neighbor] == kind) {
                    remaining.Remove(neighbor);
                    pending.Add(neighbor);
                }
            }
        }

        return result;
    }
}

