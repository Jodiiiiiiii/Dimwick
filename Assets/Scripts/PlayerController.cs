using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    [Header("Movement")]
    public float MaxWalkSpeed = 10f;
    [Tooltip("rate at which character reached max speed")]
    public float MovementSharpness = 10f;

    // components
    public Rigidbody2D rb;
    public Animator animator;

    // private variables
    private Vector2 _targetVelocity = Vector2.zero;
    private Vector2 _facing = Vector2.right;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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

        // update animator variables
        animator.SetFloat("speed", rb.velocity.sqrMagnitude);

        // update facing direction
        _facing = ((Vector2) cam.ScreenToWorldPoint(Input.mousePosition) - rb.position).normalized;

        // flip sprite based on mouse position
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("dimwick_sit")) // does not flip if sitting
        {
            if (_facing.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        
    }
}
