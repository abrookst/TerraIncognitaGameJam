using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class MapModeSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera mapCam;
    public bool mapMode = false;
    public PlayerInput playerControls;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleMap()
    {
        mapMode = !mapMode;
        mapCam.enabled = mapMode;
        playerControls.enabled = !mapMode;

        if (mapMode) {
			Cursor.lockState = CursorLockMode.None;
        } else {
			Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
