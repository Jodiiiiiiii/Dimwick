using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player;
    public Camera Cam;

    [Header("Movement")]
    public float CamMovementSpeed = 5f;
    public float CamMovementSharpness = 10f;

    [Header("Distances")]
    public float MouseOffsetFactorX = 5f;
    public float MouseOffsetFactorY = 2f;
    [Tooltip("fraction of each edge of screen which has no influence on camera movements")]
    public float MouseInfluenceRange = 1f;

    // private variables
    private Vector2 _camSpeed = Vector2.zero;
    private Vector2 _goalPosition;

    // Start is called before the first frame update
    void Start()
    {
        Cam = GetComponent<Camera>();
    }

    // using fixed update to prevent character jitter with camera movement smoothing
    void FixedUpdate()
    {
        // Calculate goal camera position (based on player and mouse positions)
        _goalPosition = Player.transform.position;
        Vector2 difference = (Vector2)(Cam.ScreenToWorldPoint(Input.mousePosition) - Player.transform.position);
        if(difference.magnitude > MouseInfluenceRange)
            _goalPosition += new Vector2(difference.normalized.x * MouseOffsetFactorX, difference.normalized.y * MouseOffsetFactorY);
       /* _goalPosition.x += InputHelper.MapRange(Mathf.Clamp(Input.mousePosition.x, MaxMouseInfluenceFactor * Screen.width, 
            (1 - MaxMouseInfluenceFactor) * Screen.width), MaxMouseInfluenceFactor * Screen.width, 
            (1 - MaxMouseInfluenceFactor) * Screen.width, -1, 1) * MouseOffsetFactorX;
        _goalPosition.y += InputHelper.MapRange(Mathf.Clamp(Input.mousePosition.y, MaxMouseInfluenceFactor * Screen.height, 
            (1 - MaxMouseInfluenceFactor) * Screen.height), MaxMouseInfluenceFactor * Screen.height, 
            (1 - MaxMouseInfluenceFactor) * Screen.height, -1, 1) * MouseOffsetFactorY;*/

        // calculate speed from goal position (camera smoothing)
        Vector2 goalCamSpeed = (_goalPosition - new Vector2(transform.position.x, transform.position.y)) * CamMovementSpeed;
        _camSpeed = Vector2.Lerp(_camSpeed, goalCamSpeed, 1 - Mathf.Exp(-CamMovementSharpness * Time.deltaTime));

        // apply velocity to change position
        transform.position += new Vector3(_camSpeed.x, _camSpeed.y, 0) * Time.deltaTime;
    }

    
}

