using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    [Header("Movement")]
    public float CamMovementSpeed = 5f;
    public float CamMovementSharpness = 10f;
    public float MouseOffsetFactorX = 5f;
    public float MouseOffsetFactorY = 2f;

    // private variables
    private Vector2 _camSpeed = Vector2.zero;
    private Vector2 _goalPosition;

    // Start is called before the first frame update
    void Start()
    {
    }

    // using fixed update to prevent character jitter with camera movement smoothing
    void FixedUpdate()
    {
        // Calculate goal camera position (based on player and mouse positions)
        _goalPosition = player.transform.position;
        _goalPosition.x += MapRange(Mathf.Clamp(Input.mousePosition.x, 0, Screen.width), 0, Screen.width, -1, 1) * MouseOffsetFactorX;
        _goalPosition.y += MapRange(Mathf.Clamp(Input.mousePosition.y, 0, Screen.height), 0, Screen.height, -1, 1) * MouseOffsetFactorY;

        // calculate speed from goal position (camera smoothing)
        Vector2 goalCamSpeed = (_goalPosition - new Vector2(transform.position.x, transform.position.y)) * CamMovementSpeed;
        _camSpeed = Vector2.Lerp(_camSpeed, goalCamSpeed, 1 - Mathf.Exp(-CamMovementSharpness * Time.deltaTime));

        // apply velocity to change position
        transform.position += new Vector3(_camSpeed.x, _camSpeed.y, 0) * Time.deltaTime;
    }

    private float MapRange(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}

