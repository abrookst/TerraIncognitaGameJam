using UnityEngine;
using System.Linq;
using System.Collections.Generic;
public class MapGenerator : MonoBehaviour
{
    int limit = 100;
    private readonly Dictionary<Vector2Int, TileType> remaining = new();
    public WorldMap map;

    void Start()
    {
            limit = 100000;
        // FIXME: needs an exclusive version
        foreach (Vector2Int pos in VectorUtils.Area(map.bounds - new Vector2Int(1, 1)))
        {
            float height = HeightAt(pos);

            if (height > 128)
            {
                remaining[pos] = TileType.Mountains;
            }
            else if (height < 64) {
                remaining[pos] = TileType.Water;
            }
            else
            {
                if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                    remaining[pos] = TileType.Plains;
                else
                    remaining[pos] = TileType.Forest;
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

        map.SpawnTiles();
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

    float HeightAt(Vector2Int pos)
    {
        return SimplexNoise.Noise.CalcPixel2D(pos.x, pos.y, 0.25f);
    }
}

