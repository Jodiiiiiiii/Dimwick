using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : MonoBehaviour
{
    [Header("Attacking")]
    public GameObject ProjectilePrefab;
    [Tooltip("layers for checking player line of sight raycast")]
    public LayerMask raycastLayerMask;
    public float FireRange = 3f;
    public float AttackCooldown = 5f;
    public float SpreadAngle = 5f;

    // private variables
    private GameObject _player;
    private CapsuleCollider2D _playerCollider;
    private float _attackCooldownTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Dimwick");
        _playerCollider = _player.GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_attackCooldownTimer > 0f)
        {
            _attackCooldownTimer -= Time.deltaTime;
        }
        else
        {
            // check for line of sight to player
            RaycastHit2D hit = Physics2D.Raycast(transform.position, 
                (Vector2)_player.transform.position + _playerCollider.offset - Vector2.up * _playerCollider.size.y - (Vector2) transform.position, 
                FireRange, raycastLayerMask);
            Debug.DrawRay(transform.position, (Vector2)_player.transform.position + _playerCollider.offset - Vector2.up * _playerCollider.size.y - (Vector2)transform.position, Color.green);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Instantiate(ProjectilePrefab, transform.position, Quaternion.Euler(0, 0, 
                    Vector2.SignedAngle(Vector2.right, (Vector2) _player.transform.position - (Vector2) transform.position)
                    + Random.Range(-SpreadAngle, SpreadAngle)));

                _attackCooldownTimer = AttackCooldown; // put attack on cooldown
            }
        }
    }
}
