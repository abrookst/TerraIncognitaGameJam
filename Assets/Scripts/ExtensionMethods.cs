using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class VectorUtils {
    public static Vector2Int Random(this Vector2Int bounds) {
        return new Vector2Int(
            UnityEngine.Random.Range(0, bounds.x),
            UnityEngine.Random.Range(0, bounds.y)
        );
    }

    public static Vector3Int XYZ(this Vector2Int pos) {
        return new Vector3Int(
            pos.x,
            0,
            pos.y
        );
    }

    public static IEnumerable<Vector2Int> Area(Vector2Int a, Vector2Int b) {
        Vector2Int lower = new(a.x < b.x ? a.x : b.x, a.y < b.y ? a.y : b.y);
        Vector2Int upper = new(a.x > b.x ? a.x : b.x, a.y > b.y ? a.y : b.y);

        for (int x = lower.x; x <= upper.x; x++) {
            for (int y = lower.y; y <= upper.y; y++) {
                yield return new(x, y);
            }
        }
    }
}