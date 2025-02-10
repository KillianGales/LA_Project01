using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    private Transform Canon, Launchpad;
    Vector3 direction, WorldMousePos;
    Quaternion targetRotation;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private ObjectPool bulletPool;
    [SerializeField] private GameObject bullet, activeBullet; 
    [SerializeField] private Transform closestEnemy; 
    [SerializeField] private int bulletLifeSpan = 4;
    private int currentBulletType= 0;
    private Enum bulletTypes;



    void Awake()
    {
        Canon = transform.Find("Canon");
        Launchpad = Canon.transform.Find("LaunchPad");
        bulletPool = gameObject.AddComponent<ObjectPool>();
        bulletPool.InitializePool(bullet, 50);
        
    }

    void OnEnable()
    {

    }
    void Start()
    {
        InvokeRepeating("AutoShoot",0f, 0.5f);
    }

    void Update()
    {
        //AutoShoot();
        //StartCoroutine(IAutoShoot());

        CanonLookat();

        if(Input.GetKey(KeyCode.Keypad1))
        {
            currentBulletType = 0;
        }
        else if(Input.GetKey(KeyCode.Keypad2))
        {
            currentBulletType = 1;
        }

    }

    private void CanonLookat()
    {
        WorldMousePos = InputManager.GetWorldMousePosition();

        direction = WorldMousePos - Canon.position;
        direction.y = 0;

        targetRotation = Quaternion.LookRotation(direction);
        Canon.rotation = Quaternion.Slerp(Canon.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private IEnumerator IAutoShoot()
    {
        activeBullet = bulletPool.GetObjectFromPool(Launchpad.position);
        yield return new WaitForSeconds(1f);
        
    }

    private void AutoShoot()
    {
        activeBullet = bulletPool.GetObjectFromPool(Launchpad.position);
        Bullet bullet = activeBullet.GetComponent<Bullet>();
        bullet.Initialize(currentBulletType, WorldMousePos);
        StartCoroutine(PoolReset(activeBullet));
    }

    private IEnumerator PoolReset(GameObject currentBullet)
    {
        yield return new WaitForSeconds(bulletLifeSpan);
        bulletPool.ReturnObject(currentBullet);
    }

}
