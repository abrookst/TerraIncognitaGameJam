using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridMovement : MonoBehaviour
{
    public Vector2Int destination = new(3,3);
    private Vector3 velocity;

    public Vector2Int orientation = Vector2Int.up;
    public float angleVelocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = WorldMap.instance.GetPosFor(destination);
        Vector3 result = Vector3.SmoothDamp(transform.position, target, ref velocity, 0.1f);

        result.y = 0;

        result = WorldMap.instance.AddTerrainHeight(result);

        transform.position = result;

        Quaternion angleTarget = Quaternion.Euler(0, -180 / Mathf.PI * Mathf.Atan2(orientation.y, orientation.x), 0);

        float delta = Quaternion.Angle(transform.rotation, angleTarget);
        if (delta > 0f)
        {
            float t = Mathf.SmoothDampAngle(delta, 0.0f, ref angleVelocity, 0.1f);
            t = 1.0f - (t / delta);
            transform.rotation = Quaternion.Slerp(transform.rotation, angleTarget, t);
        }
    }

    public void Move(Vector2Int delta) {
        Vector2Int target = destination + delta;

        if ((WorldMap.instance.GetPosFor(destination).XZ() - transform.position.XZ()).magnitude > 0.1f)
            return;
        
        if (WorldMap.instance.map[target].Passable(target)) {
            destination = target;
        }
    }

    public void MoveForward(InputAction.CallbackContext context) {
        if (context.performed)
            Move(orientation);
    }

    public void MoveBack(InputAction.CallbackContext context) {
        if (context.performed)
            Move(-1 * orientation);
    }

    public void RotateLeft(InputAction.CallbackContext context) {
        if (context.performed)
            orientation = new Vector2Int(-orientation.y, orientation.x);
    }

    public void RotateRight(InputAction.CallbackContext context) {
        if (context.performed)
            orientation = new Vector2Int(orientation.y, -orientation.x);
    }
}
