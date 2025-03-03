using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TurretBehaviour : MonoBehaviour
{
    [SerializeField] private Transform Canon, Launchpad, turretBase;
    [SerializeField] List<Transform> sockets;
    Vector3 direction, WorldMousePos;
    Quaternion targetRotation;
    [SerializeField] private float rotationSpeed, autoRotationSpeed;
    [SerializeField] private ObjectPool bulletPool;
    [SerializeField] private GameObject bullet/*, activeBullet*/; 
    [SerializeField] private Transform closestEnemy; 
    [SerializeField] private int bulletLifeSpan = 10;
    //private int currentBulletType= 0;
    private EmodTypes bulletType;
    [SerializeField]private float fireRate, baseFireRate;
    public List<GameObject> activeMods;
    public GameObject Mod1;
    public LayerMask pickupLayer;
    public float life, baseLife;
    public Image lifeRep;


    void Awake()
    {
        Canon = transform.Find("Canon");
        Launchpad = Canon.transform.Find("LaunchPad");
        turretBase = transform.Find("Base");

        for (int i = 0; i < turretBase.childCount; i++)
        {
            sockets.Add(turretBase.GetChild(i));
        }

        life = baseLife;
        

        bulletPool = gameObject.AddComponent<ObjectPool>();
        bulletPool.InitializePool(bullet, 50);
    }

    
    void Start()
    {
        GameManager.Instance.AddObject(transform);

        for (int i = 0; i<activeMods.Count; i++)
        {
            if(activeMods[i] != null)
            {
                InitBulletType(i, true);
            }
        }

        //StartCoroutine(EAutoShoot());
    }

    private void InitBulletType(int i, bool isStart)
    {
        Debug.Log(i);
        if (activeMods.Count > 0)
        {
            Transform curMod;
            if(isStart)
            {
                curMod = Instantiate(activeMods[i]).transform;
            }
            else
            {
                curMod = activeMods[i].transform;
            }

            curMod.position = sockets[i].transform.position;
            curMod.rotation = sockets[i].transform.rotation;
            curMod.transform.SetParent(sockets[i].transform);

            ModProfile curModData = curMod.GetComponent<ModProfile>();

            switch (curModData.type)
            {
                case EmodTypes.CanonBall:
                    //currentBulletType = 2;
                    // fireRate = curModData.fireRate;
                    StartCoroutine(EAutoShoot(curModData));
                    Debug.Log(name + "is set to fire CanonBall");
                    break;
                case EmodTypes.FastBullet:
                    //currentBulletType = 1;
                    //fireRate = curModData.fireRate;
                    StartCoroutine(EAutoShoot(curModData));
                    Debug.Log(name + "is set to fire FastBullet");
                    break;
            }

        }
    }



    void Update()
    {
        //AutoShoot();
        CanonLookat();
        if(life>0)
        {
            lifeRep.fillAmount = life/baseLife;
        }
        else
        {
            life = 0;
        }


        if(GameManager.Instance.droppedMods.Count > 0)
        {
            CheckForMod();
        }

       /* if(GameManager.Instance.TurnAct)
        {
            TurnAct();
        }*/


    }

    private void CheckForMod()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, pickupLayer))
        {
            GameObject obj = hit.collider.gameObject;

            for (int i = 0; i < GameManager.Instance.droppedMods.Count; i++)
            {

                if (obj == GameManager.Instance.droppedMods[i]&& activeMods.Count<3)
                {
                    obj.GetComponent<ModProfile>().dropped = false;
                    ModPickup(obj);
                    GameManager.Instance.droppedMods.Remove(obj);
                }

            }

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

    private IEnumerator EAutoShoot(ModProfile curModData)
    {
        GameObject activeBullet;
        Bullet Ibullet;
        while(true)
        {
            fireRate = curModData.bulletType.fireRate;

            activeBullet = bulletPool.GetObjectFromPool(Launchpad.position);
            if(!activeBullet)break;
            Ibullet = activeBullet.GetComponent<Bullet>();
            Ibullet.Initialize(Launchpad.forward, curModData.bulletType);
            StartCoroutine(PoolReset(activeBullet));

            //AutoShoot();
            yield return new WaitForSeconds(fireRate);
        }

        
    }

    void AutoRotate(float speed)
    {
        Canon.rotation = Quaternion.Slerp(Canon.rotation,Quaternion.LookRotation(Launchpad.right), speed* Time.deltaTime);
    }
/*
    private void AutoShoot(int bulletType, ModProfile curModData)
    {
        currentBulletType = bulletType;
        fireRate = curModData.fireRate;
        

        activeBullet = bulletPool.GetObjectFromPool(Launchpad.position);
        if(!activeBullet)return;
        Bullet bullet = activeBullet.GetComponent<Bullet>();
        bullet.Initialize(currentBulletType, Launchpad.forward);
        StartCoroutine(PoolReset(activeBullet));
    }
*/
    private IEnumerator PoolReset(GameObject currentBullet)
    {
        yield return new WaitForSeconds(bulletLifeSpan);
        bulletPool.ReturnObject(currentBullet);
    }

 /*   void TurnAct()
    {
        AutoShoot();
    }
*/
    void ModPickup(GameObject mod)
    {
        mod.layer = 0;
        activeMods.Add(mod);
        InitBulletType(activeMods.Count-1, false);
    }

}
