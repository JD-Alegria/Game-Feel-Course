using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Pool;
using Cinemachine;

public class Gun : MonoBehaviour
{
    Animator _animator;
    CinemachineImpulseSource _cinemachineSource;

    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] float gunFireCD = .5f;

    ObjectPool<Bullet> _bulletPool;
    static readonly int FIRE_HASH = Animator.StringToHash("Fire");
    Vector2 mousePos;
    float lastFireTime = 0f;

    public static event Action OnShoot;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _cinemachineSource = GetComponent<CinemachineImpulseSource>();
    }
    
    private void Update()
    {
        Shoot();
        RotateGun();
    }

    void Start()
    {
        CreateBulletPool();
    }

    void OnEnable()
    {
        OnShoot += ShootProjectile;
        OnShoot += ResetLastFireTime;
        OnShoot += FireAnimation;
        OnShoot += GunScreenShake;
    }

    void OnDisable()
    {
        OnShoot -= ShootProjectile;
        OnShoot -= ResetLastFireTime;
        OnShoot -= FireAnimation;
        OnShoot -= GunScreenShake;
    }

    void CreateBulletPool()
    {
        _bulletPool = new ObjectPool<Bullet>(() => { return Instantiate(_bulletPrefab); },
            bullet => { bullet.gameObject.SetActive(true); },
            bullet =>
            {
                bullet.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                bullet.gameObject.SetActive(false);
            },
            bullet => { Destroy(bullet.gameObject); });
    }

    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }

    private void Shoot()
    {
        if (Input.GetMouseButton(0) && Time.time >= lastFireTime) 
        {
            OnShoot?.Invoke();
            
            // animation
            // sfx
            // screen shake
            // muzzle flash
        }
    }

    private void ShootProjectile()
    {
        if (Time.time < lastFireTime) return;

        Bullet newBullet = _bulletPool.Get();
        newBullet.Init(this, _bulletSpawnPoint.position, mousePos);
    }

    void ResetLastFireTime()
    {
        lastFireTime = Time.time + gunFireCD;
    }

    void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0f);
    }

    void GunScreenShake()
    {
        _cinemachineSource.GenerateImpulse();
    }

    void RotateGun()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = PlayerController.Instance.transform.InverseTransformPoint(mousePos);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
