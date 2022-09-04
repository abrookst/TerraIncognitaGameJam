using UnityEngine;
using System.Collections.Generic;

public abstract class Landmark {

    public abstract void Generate(Transform root, Vector2Int coord);
    public virtual Vector3 AddHeight(Vector3 pos) {
        return pos;
    }
}