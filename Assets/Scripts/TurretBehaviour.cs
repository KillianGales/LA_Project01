using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    private Transform Canon, Launchpad;
    Vector3 direction, WorldMousePos;
    Quaternion targetRotation;
    [SerializeField] private float rotationSpeed, autoRotationSpeed;
    [SerializeField] private ObjectPool bulletPool;
    [SerializeField] private GameObject bullet, activeBullet; 
    [SerializeField] private Transform closestEnemy; 
    [SerializeField] private int bulletLifeSpan = 10;
    private int currentBulletType= 0;
    private Enum bulletTypes;
    [SerializeField]private float fireRate, baseFireRate;


    void Awake()
    {
        Canon = transform.Find("Canon");
        Launchpad = Canon.transform.Find("LaunchPad");

        bulletPool = gameObject.AddComponent<ObjectPool>();
        bulletPool.InitializePool(bullet, 50);
        //GameManager.Instance.AddObject(transform);
    }

    void OnEnable()
    {
        //GameManager.Instance.AddObject(transform);
    }
    void Start()
    {
        GameManager.Instance.AddObject(transform);
        fireRate = baseFireRate;
        //InvokeRepeating("AutoShoot",0f, 0.1f);
        StartCoroutine(EAutoShoot());
    }

    void Update()
    {
        //AutoShoot();
        //StartCoroutine(EAutoShoot());
        CanonLookat();
        //AutoRotate(autoRotationSpeed);

        if(Input.GetKey(KeyCode.Keypad1))
        {
            currentBulletType = 0;
            fireRate = baseFireRate;
        }
        else if(Input.GetKey(KeyCode.Keypad2))
        {
            currentBulletType = 1;
            fireRate = baseFireRate / 5f;
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

    private IEnumerator EAutoShoot()
    {
        while(true)
        {
            AutoShoot();
            yield return new WaitForSeconds(fireRate);
        }

        
    }

    void AutoRotate(float speed)
    {
        Canon.rotation = Quaternion.Slerp(Canon.rotation,Quaternion.LookRotation(Launchpad.right), speed* Time.deltaTime);
    }

    private void AutoShoot()
    {
        activeBullet = bulletPool.GetObjectFromPool(Launchpad.position);
        if(!activeBullet)return;
        Bullet bullet = activeBullet.GetComponent<Bullet>();
        bullet.Initialize(currentBulletType, Launchpad.forward);
        StartCoroutine(PoolReset(activeBullet));
    }

    private IEnumerator PoolReset(GameObject currentBullet)
    {
        yield return new WaitForSeconds(bulletLifeSpan);
        bulletPool.ReturnObject(currentBullet);
    }

}
