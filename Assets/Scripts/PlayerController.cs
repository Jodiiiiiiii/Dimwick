using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MaxWalkSpeed = 10f;
    [Tooltip("rate at which character reached max speed")]
    public float MovementSharpness = 10f;

    // components
    public Rigidbody2D rb;

    // private variables
    private Vector2 _targetVelocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Handle movement direction input
        switch(InputHelper.GetOctoDirectionHeld())
        {
            case InputHelper.OctoDirection.Up:
                _targetVelocity = Vector2.up * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.Right:
                _targetVelocity = Vector2.right * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.Down:
                _targetVelocity = Vector2.down * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.Left:
                _targetVelocity = Vector2.left * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.UpRight:
                _targetVelocity = (Vector2.up + Vector2.right).normalized * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.DownRight:
                _targetVelocity = (Vector2.down + Vector2.right).normalized * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.DownLeft:
                _targetVelocity = (Vector2.down + Vector2.left).normalized * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.UpLeft:
                _targetVelocity = (Vector2.up + Vector2.left).normalized * MaxWalkSpeed;
                break;
            case InputHelper.OctoDirection.None:
                _targetVelocity = Vector2.zero;
                break;
        }

        // update velocity based on target and current velocities
        rb.velocity = Vector2.Lerp(rb.velocity, _targetVelocity, 1 - Mathf.Exp(-MovementSharpness * Time.deltaTime));
    }
}
