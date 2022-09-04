//Attach this script to your Canvas GameObject.
//Also attach a GraphicsRaycaster component to your canvas by clicking the Add Component button in the Inspector window.
//Also make sure you have an EventSystem in your hierarchy.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using System;

public enum MapMarkingType
{
    Empty,
    Grassy,
    Rocky,
    Sandy,
    Watery
}

public enum MapStampType
{
    Mountain,
    Building
}

public enum MapDrawMode
{
    Marker,
    Fill,
    Pencil
}

public class MapController : MonoBehaviour
{

    public Dictionary<Vector2Int, MapMarkingType> markings = new();
    public Dictionary<Vector2, MapStampType> stamps = new();
    public Dictionary<MapMarkingType, Color> markingColors = new() {
        { MapMarkingType.Empty, new Color(1, 1, 1, 0)},
        { MapMarkingType.Grassy, Color.Lerp(Color.black, Color.green, 0.5f)},
        { MapMarkingType.Rocky, Color.red },
        { MapMarkingType.Sandy, Color.yellow },
        { MapMarkingType.Watery, Color.blue }
    };

    public bool initialized = false;

    public Vector2Int dimensions;
    RectTransform rect;
    RawImage image;
    RawImage grid;
    RawImage pencil;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    public Texture2D texture;
    public Texture2D pencilTexture;

    public MapMarkingType currentMarking;
    public MapStampType currentStamp;
    public MapDrawMode currentMode = MapDrawMode.Marker;
    private MapMarkingType drawingOver;

    public TMP_Text modeText;
    public TMP_Text scoreTracker;

    public RectTransform markerColorPicker;
    public GameObject markerColorOption;

    private Dictionary<MapMarkingType, Toggle> markerOptions = new();
    void Awake()
    {
        texture = new(dimensions.x, dimensions.y)
        {
            filterMode = FilterMode.Point
        };
    }
    void Start()
    {
        foreach (MapMarkingType marker in Enum.GetValues(typeof(MapMarkingType))) {
            GameObject option = Instantiate(markerColorOption, markerColorPicker);
            option.transform.Find("Background").GetComponent<Image>().color = markingColors[marker];
            Toggle toggle = option.GetComponent<Toggle>();
            
            toggle.onValueChanged.AddListener(value => {
                if (value)
                    currentMarking = marker;
            });

            toggle.group = markerColorPicker.GetComponent<ToggleGroup>();

            markerOptions[marker] = toggle;
        }

        rect = transform.Find("Image").GetComponent<RectTransform>();
        image = transform.Find("Image").GetComponent<RawImage>();
        grid = transform.Find("Grid").GetComponent<RawImage>();
        pencil = transform.Find("Pencil").GetComponent<RawImage>();
        image.texture = texture;
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();

        Texture2D gridTex = new(dimensions.x * 10, dimensions.y * 10, TextureFormat.RGBA32, 0, true)
        {
            filterMode = FilterMode.Point
        };

        pencilTexture = new(dimensions.x * 10, dimensions.y * 10, TextureFormat.RGBA32, 0, true)
        {
            filterMode = FilterMode.Point
        };

        pencil.texture = pencilTexture;

        byte[] textureData = new byte[100 * dimensions.x * dimensions.y * 4];

        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                markings[new Vector2Int(x, y)] = MapMarkingType.Empty;
                texture.SetPixel(x, y, markingColors[MapMarkingType.Empty]);
            }
        }
        texture.Apply();

        grid.texture = gridTex;

        int index = 0;
        for (int x = 0; x < 10 * dimensions.x; x++)
        {
            for (int y = 0; y < 10 * dimensions.y; y++)
            {
                byte color = (byte)((x % 10 == 0 || x % 10 == 9 || y % 10 == 0 || y % 10 == 9) ? 0 : 255);
                textureData[index * 4] = color;
                textureData[index * 4 + 1] = color;
                textureData[index * 4 + 2] = color;
                textureData[index * 4 + 3] = (byte) (color == 0 ? 64 : 0);
                ++index;
            }
        }

        gridTex.SetPixelData(textureData, 0);
        gridTex.Apply();

        for (int i = 0; i < dimensions.x * dimensions.y * 100 * 4; i++) {
            textureData[i] = 0;
        }

        pencilTexture.SetPixelData(textureData, 0);
        pencilTexture.Apply();
        UseMarking(0);
    }

    void Update()
    {
        if (WorldMap.instance != null) {
            if (!initialized) {
                WorldMap.instance.FillMap(this);
                initialized = true;
            }

            scoreTracker.text = Mathf.Round(WorldMap.instance.ScoreMap(this) * 100).ToString() + "%";
        }
        modeText.text = currentMode.ToString();

        bool started = Input.GetKeyDown(KeyCode.Mouse0);
        //Check if the left Mouse button is clicked
        if (Input.GetKey(KeyCode.Mouse0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new(m_EventSystem)
            {
                //Set the Pointer Event Position to that of the mouse position
                position = Input.mousePosition
            };

            //Create a list of Raycast Results
            List<RaycastResult> results = new();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                Debug.Log("Hit " + result.gameObject.name);

                if (result.gameObject.name == "Image")
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,
                    result.screenPosition, Camera.main, out Vector2 localPosition);
                    Debug.Log(localPosition);
                    localPosition += rect.rect.size / 2;
                    localPosition.Scale(new Vector2(1 / rect.rect.width, 1 / rect.rect.height));

                    if (currentMode != MapDrawMode.Pencil) {
                        localPosition.Scale(dimensions);
                    } else {
                        localPosition.Scale(dimensions * 10);
                    }
                    Vector2Int spot = new(Mathf.FloorToInt(localPosition.x), Mathf.FloorToInt(localPosition.y));

                    if (currentMode == MapDrawMode.Marker)
                    {
                        if (started)
                            drawingOver = markings[spot];

                        if (markings[spot] == drawingOver)
                        {
                            DrawOnMap(spot);
                            texture.Apply();
                        }
                    }
                    else if (currentMode == MapDrawMode.Fill)
                    {
                        // Only fill if it's a different color!
                        if (markings[spot] != currentMarking)
                        {
                            MapMarkingType target = markings[spot];
                            DrawOnMap(spot);
                            FloodFill(spot, target);
                            texture.Apply();

                        }
                    }
                    else
                    {
                        DrawWithPencil(spot);
                        pencilTexture.Apply();
                    }
                }
            }
        }
    }

    public void Refresh()
    {
        foreach (Vector2Int pos in markings.Keys) {
            texture.SetPixel(pos.x, pos.y, markingColors[markings[pos]]);
        }

        texture.Apply();
    }

    void DrawOnMap(Vector2Int spot)
    {
        markings[spot] = currentMarking;
        texture.SetPixel(spot.x, spot.y, markingColors[currentMarking]);
    }

    void DrawWithPencil(Vector2Int spot)
    {
        pencilTexture.SetPixel(spot.x, spot.y, Color.black);
    }

    void FloodFill(Vector2Int spot, MapMarkingType target)
    {
        foreach (Vector2Int dir in new[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left })
        {
            Vector2Int neighbor = spot + dir;
            if (markings.ContainsKey(neighbor) && markings[neighbor] == target)
            {
                DrawOnMap(neighbor);
                FloodFill(neighbor, target);
            }
        }
    }

    public void UseMarking(int index)
    {
        this.currentMarking = (MapMarkingType)index;
        markerOptions[currentMarking].isOn = true;
    }

    public void SwitchMode(InputAction.CallbackContext context)
    {
        if (context.performed) {
            this.currentMode = (MapDrawMode)(((int)this.currentMode + 1) % 3);
        }
        // TODO sound effect!!!
    }

}