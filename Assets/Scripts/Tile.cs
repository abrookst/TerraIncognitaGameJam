using UnityEngine;
using System.Collections.Generic;

// Better named as a *collection* of tiles. This class holds
// one or more coordinates. The coordinates are adjacent to
// each other, ignoring diagonals.

// A Tile is responsible for generating its features: e.g. a
// mountain peak, or a collection of trees.

// Tiles can report if they're passable or not, and can also
// report the height at any point. This is useful for
// overriding the actual terrain -- e.g. for frozen water.

public abstract class Tile {
    public List<Vector2Int> coordinates = new();
    public Dictionary<Vector2Int, List<GameObject>> features = new();

    public MapMarkingType marking;
    public virtual bool Passable(Vector2Int pos) {
        return true;
    }

    public virtual Vector3 AddHeight(Vector3 worldPos) {
        return new Vector3(worldPos.x, Terrain.activeTerrain.SampleHeight(worldPos), worldPos.z);
    }

    public abstract void Generate(Transform root);

    public Tile(IEnumerable<Vector2Int> coordinates) {
        this.coordinates = new List<Vector2Int>(coordinates);

        foreach(Vector2Int pos in coordinates) {
            features[pos] = new List<GameObject>();
        }
    }

    public bool Contains(Vector3 worldPos) {
        return coordinates.Contains(WorldMap.instance.GetCoordFor(worldPos.XZ()));
    }
}