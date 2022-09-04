using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Map Config", menuName = "Map Config")]
public class MapConfig : ScriptableObject {
    public float timeLimit = 300f;
    public Vector2Int bounds;
    public AnimationCurve elevationCurve;
    public float minElevation;
    public float maxElevation;
    
    public float minMoisture;
    public float maxMoisture;

    public float minTemperature;
    public float maxTemperature;

    public float HeightAt(Vector2 pos)
    {
        SimplexNoise.Noise.Seed = 1337 + WorldMap.instance.seed;
        float raw = SimplexNoise.Noise.CalcPixel2D(Mathf.FloorToInt(1000 * pos.x), Mathf.FloorToInt(1000 * pos.y), 0.00002f);
        raw /= 255;
        
        raw = elevationCurve.Evaluate(raw);
        return Mathf.Lerp(minElevation, maxElevation, raw);
    }

    public float HeightAt(Vector3 pos)
    {
        throw new System.Exception("You gave a Vector3 to a Vector2, dumbass");
    }

    public float MoistureAt(Vector2 pos)
    {
        SimplexNoise.Noise.Seed = 420 + WorldMap.instance.seed;
        float raw = SimplexNoise.Noise.CalcPixel2D(Mathf.FloorToInt(1000 * pos.x), Mathf.FloorToInt(1000 * pos.y), 0.00002f);
        raw /= 255;
        return Mathf.Lerp(minMoisture, maxMoisture, raw);
    }

    public float TemperatureAt(Vector2 pos)
    {
        SimplexNoise.Noise.Seed = 42069 + WorldMap.instance.seed;
        float raw = SimplexNoise.Noise.CalcPixel2D(Mathf.FloorToInt(1000 * pos.x), Mathf.FloorToInt(1000 * pos.y), 0.00002f);
        raw /= 255;
        return Mathf.Lerp(minTemperature, maxTemperature, raw);
    }
}