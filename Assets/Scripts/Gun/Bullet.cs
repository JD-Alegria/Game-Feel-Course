using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private int _damageAmount = 1;
    Gun _gun;
    bool isReleased = false;

    private Vector2 _fireDirection;

    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    public void Init(Gun gun, Vector2 bulletSpawnPos, Vector2 mosPos)
    {
        _gun = gun;
        _fireDirection = (mosPos - bulletSpawnPos);
        _fireDirection.Normalize();
        isReleased = false;
        
        gameObject.transform.position = bulletSpawnPos;
        transform.rotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = _fireDirection * _moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == gameObject.layer) return;
        if (isReleased) return;
        
        Health health = other.gameObject.GetComponent<Health>();
        health?.TakeDamage(_damageAmount);
        isReleased = true;
        _gun.ReleaseBulletFromPool(this);
    }
}