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
    //private int currentBulletType= 0;
    private EmodTypes bulletType;
    [SerializeField]private float fireRate, baseFireRate;
    public List<ModProfile> activeMods;
    public ModProfile newMod;
    public LayerMask pickupLayer;
    public float life, baseLife;
    public Image lifeRep;
    public List<Coroutine> shootRoutines = new List<Coroutine>(new Coroutine[3]);
    private bool checkingForMods;
    [SerializeField] public Dictionary<ModProfile,Coroutine> activeModRoutine = new Dictionary<ModProfile,Coroutine>();
    public Transform outOfBoundsBulletPool;

    [Header("UI Setup")]
    [SerializeField] private List<Image> modImages;
    [SerializeField] private GameObject currentModCanvas, NewModCanvas, pauseButton;
    [SerializeField] private Image newModImage;

    [Header("Laser Setup")]
    public GameObject laserObj;
    private LineRenderer laserLine;
    public float maxLength = 20f;
    public float growSpeed = 40f;
    public LayerMask hitLayers;
    private float currentLength = 0f;
    //private bool isFiring = false;
    public float moveSpeed;
    public Rigidbody rb;
    private List<Transform> enemies;
    public float autoTargetingRange;

    void Awake()
    {
        Canon = transform.Find("Canon");
        Launchpad = Canon.transform.Find("LaunchPad");
        turretBase = transform.Find("Base");
        laserLine = laserObj.GetComponent<LineRenderer>();

        for (int i = 0; i < turretBase.childCount; i++)
        {
            sockets.Add(turretBase.GetChild(i));
            //activeModRoutine.Add(StartCoroutine("EmptyRoutine"));
        }

        life = baseLife;
        checkingForMods = true;
        
        currentModCanvas.SetActive(false);

        bulletPool = gameObject.AddComponent<ObjectPool>();
        bulletPool.InitializePool(bullet, 50, outOfBoundsBulletPool);
    }

    
    void Start()
    {
        GameManager.Instance.AddObject(transform);

        for (int i = 0; i<GameManager.Instance.startingMods.Count; i++)
        {
            if(GameManager.Instance.startingMods[i] != null)
            {
                activeMods.Add(GameManager.Instance.startingMods[i]);
                InitBulletType(activeMods[i], true, i);
            }
        }

        //StartCoroutine(EAutoShoot());
    }

    private void InitBulletType(ModProfile curModData, bool isStart, int index)
    {

        if (activeMods.Count > 0)
        {
            Transform curMod;
            Transform origin = null;
            if(isStart)
            {
                curMod = Instantiate(curModData.gameObject).transform;
            }
            else
            {
                Debug.Log("New mod is checked at index " + index);
                //activeMods.Add(curModData);
                curMod = activeMods[index].transform;
            }

            curMod.position = sockets[index].transform.position;
            curMod.rotation = sockets[index].transform.rotation;
            curMod.transform.SetParent(sockets[index].transform);

            switch(curModData.origin)
            {
                case 0:
                    origin = Launchpad;
                    break;
                case 1 :
                    origin = curMod;
                    break;

            }

            SetModUI(index, curModData.bulletType);

            if(shootRoutines[index] != null) StopCoroutine(shootRoutines[index]);

            switch(index)
            {
                case 0 :
                    //StopCoroutine(shootRoutines[0]);
                    shootRoutines[0] = StartCoroutine(EAutoShoot00(curModData, origin));
                    break;
                case 1 :
                    //StopCoroutine(shootRoutines[1]);
                    shootRoutines[1] = StartCoroutine(EAutoShoot01(curModData, origin));
                    break;
                case 2 :
                    //StopCoroutine(shootRoutines[2]);
                    shootRoutines[2] = StartCoroutine(EAutoShoot02(curModData, origin));
                    break;
            }

            newMod = null;
        }
    }
    

    void Update()
    {
        //AutoShoot();
        //GetClosestEnemy();
        CanonLookat();
        MoveAround();
        //AutoRotate(2.5f);

        if(life>0)
        {
            lifeRep.fillAmount = life/baseLife;
        }
        else
        {
            life = 0;
        }

/*
        if(GameManager.Instance.droppedMods.Count > 0 && checkingForMods)
        {
            CheckForMod();
        }*/

    }

    private void CheckForMod()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, pickupLayer))
        {
            GameObject obj = hit.collider.gameObject;

            GetMod(obj);

        }
    }
    void OnTriggerEnter(Collider other)
    {
        
        GameObject obj = other.gameObject;

        if(obj.layer == 6)
        {
            
            GetMod(obj);
        }
    }

    public void GetMod(GameObject obj)
    {
       /* for (int i = 0; i < GameManager.Instance.droppedMods.Count; i++)
            {
                if(obj == GameManager.Instance.droppedMods[i])
                {*/
                    newMod = obj.GetComponent<ModProfile>();
                    newMod.dropped = false;
                    GameManager.Instance.droppedMods.Remove(obj);

                    if(activeMods.Count<3)
                    {
                        ModPickup(newMod);
                    }
                    else if (activeMods.Count>=3)
                    {
                        GameManager.Instance.TimePause();
                        checkingForMods = false;
                        UIDisplayNewMod(newMod.bulletType);
                        DeactivatePauseButton();
                        ActivateNewModCanvas();
                        ActivateCurrentModCanvas();
                    }
              /*  }
            }*/
    }

    Transform GetClosestEnemy()
    {
        enemies = SpawnerManager.instance.spawnedEnemyRef;

        if(enemies.Count <= 0) return null;

        //Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue; // Ignore destroyed enemies
            float sqrDistance = (enemy.position - currentPosition).sqrMagnitude;
            if (sqrDistance < closestDistanceSqr && sqrDistance < autoTargetingRange)
            {
                closestDistanceSqr = sqrDistance;
                closestEnemy = enemy;
                ///Debug.Log(sqrDistance);
            }
            else if (sqrDistance < closestDistanceSqr && sqrDistance > autoTargetingRange)
            {
                closestEnemy = null;
            }
        }

        return closestEnemy;
    }

    private void CanonLookat()
    {
        GetClosestEnemy();

        if(enemies.Count == 0 || closestEnemy == null) return;
        

        Vector3 orientation = closestEnemy.position-transform.position;

        targetRotation = Quaternion.LookRotation(new Vector3(orientation.x, 0, orientation.z)/*.normalized*/);
        Canon.rotation = Quaternion.Slerp(Canon.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void MoveAround()
    {
        WorldMousePos = InputManager.GetWorldMousePosition();

        direction = WorldMousePos - transform.position;
        direction.y = 0;

        if(enemies.Count == 0 || closestEnemy == null ) 
        {
            targetRotation = Quaternion.LookRotation(direction);
            Canon.rotation = Quaternion.Slerp(Canon.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        //Logic to direct fire towards mousePos
        //targetRotation = Quaternion.LookRotation(-direction);
        //Canon.rotation = Quaternion.Slerp(Canon.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        direction = direction.normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

/// <summary>
/// //Start of Coroutines section//////
/// </summary>
//</param>

    private IEnumerator EAutoShoot00(ModProfile curModData, Transform origin)
    {
        GameObject activeBullet;
        Bullet Ibullet;

        if(curModData.bulletType.behaviour == EBehaviour.Laser)
        {
            laserLine.enabled = true;
            currentLength = 0f;

            while (true)
            {
                if(currentLength < maxLength)
                    currentLength += growSpeed * Time.deltaTime;
                Ibullet = laserObj.GetComponent<Bullet>();
                UpdateLaser(Ibullet);
                yield return null;
            }
        }
        else
        {
            while(true)
            {
                fireRate = curModData.bulletType.fireRate;

                activeBullet = bulletPool.GetObjectFromPool(origin.position);
                if(!activeBullet)break;
                Ibullet = activeBullet.GetComponent<Bullet>();
                Ibullet.Initialize(origin, curModData.bulletType);
                StartCoroutine(PoolReset(activeBullet, Ibullet.lifeSpan));

            //AutoShoot();
                yield return new WaitForSeconds(fireRate);
            }
        }  
    }

    private IEnumerator EAutoShoot01(ModProfile curModData, Transform origin)
    {
        GameObject activeBullet;
        Bullet Ibullet;

        if(curModData.bulletType.behaviour == EBehaviour.Laser)
        {
            laserLine.enabled = true;
            currentLength = 0f;

            while (true)
            {
                if(currentLength < maxLength)
                    currentLength += growSpeed * Time.deltaTime;
                Ibullet = laserObj.GetComponent<Bullet>();
                UpdateLaser(Ibullet);
                yield return null;
            }
        }
        else
        {
            while(true)
            {
                fireRate = curModData.bulletType.fireRate;

                activeBullet = bulletPool.GetObjectFromPool(origin.position);
                if(!activeBullet)break;
                Ibullet = activeBullet.GetComponent<Bullet>();
                Ibullet.Initialize(origin, curModData.bulletType);
                StartCoroutine(PoolReset(activeBullet, Ibullet.lifeSpan));

            //AutoShoot();
                yield return new WaitForSeconds(fireRate);
            }
        }  
    }

    private IEnumerator EAutoShoot02(ModProfile curModData,Transform origin)
    {
        GameObject activeBullet;
        Bullet Ibullet;

        if(curModData.bulletType.behaviour == EBehaviour.Laser)
        {
            laserLine.enabled = true;
            currentLength = 0f;

            while (true)
            {
                if(currentLength < maxLength)
                    currentLength += growSpeed * Time.deltaTime;
                Ibullet = laserObj.GetComponent<Bullet>();
                UpdateLaser(Ibullet);
                yield return null;
            }
        }
        else
        {
            while(true)
            {
                fireRate = curModData.bulletType.fireRate;

                activeBullet = bulletPool.GetObjectFromPool(origin.position);
                if(!activeBullet)break;
                Ibullet = activeBullet.GetComponent<Bullet>();
                Ibullet.Initialize(origin, curModData.bulletType);
                StartCoroutine(PoolReset(activeBullet, Ibullet.lifeSpan));

            //AutoShoot();
                yield return new WaitForSeconds(fireRate);
            }
        }  
    }


    /*
    private IEnumerator EAutoShoot02(ModProfile curModData,Transform origin)
    {
        GameObject activeBullet;
        Bullet Ibullet;
        while(true)
        {
            fireRate = curModData.bulletType.fireRate;

            activeBullet = bulletPool.GetObjectFromPool(origin.position);
            if(!activeBullet)break;
            Ibullet = activeBullet.GetComponent<Bullet>();
            Ibullet.Initialize(origin, curModData.bulletType);
            StartCoroutine(PoolReset(activeBullet, Ibullet.lifeSpan));

            //AutoShoot();
            yield return new WaitForSeconds(fireRate);
        }  
    }*/

/// <summary>
/// //End of Coroutines section//////
/// </summary>
//</param>
private float lastDamageTime = 0f;
    void UpdateLaser(Bullet Ibullet)
    {
        RaycastHit hit;
        Vector3 startPos = Launchpad.position;
        Vector3 endPos = Launchpad.position + Launchpad.forward * currentLength;

        if (Physics.Raycast(startPos, Launchpad.forward, out hit, currentLength, hitLayers))
        {
            endPos = hit.point;
            Enemy enemyHit = hit.transform.GetComponent<Enemy>();
            
            if (enemyHit != null && Time.time - lastDamageTime >= Ibullet.myType.fireRate)
            {
                lastDamageTime = Time.time; // Update the timer
                enemyHit.EvaluateDamage(Ibullet);
            }
        }

        laserLine.SetPosition(0, startPos);
        laserLine.SetPosition(1, endPos);
    }
    

    void AutoRotate(float speed)
    {
        Canon.rotation = Quaternion.Slerp(Canon.rotation,Quaternion.LookRotation(Launchpad.up), speed* Time.deltaTime);
    }

    private IEnumerator PoolReset(GameObject currentBullet, float bulletLifeSpan)
    {
        yield return new WaitForSeconds(bulletLifeSpan);
        bulletPool.ReturnObject(currentBullet);
    }

    void ModPickup(ModProfile mod)
    {
        mod.gameObject.layer = 0;
        activeMods.Add(mod);

        InitBulletType(mod, false, activeMods.Count-1);
        Debug.Log("Starting the " + (activeMods.Count-1) + "th coroutine");
        
    }
    public void OpenPauseMenu()
    {
        checkingForMods = false;
        ActivateCurrentModCanvas();
        Time.timeScale = 0.0f;
    }
    public void ClosePauseMenu()
    {
        checkingForMods = true;
        DeactivateCurrentModCanvas();
        Time.timeScale = 1.0f;
    }

    public void OpenSwitchMenu()
    {
        checkingForMods = false;
        ActivateCurrentModCanvas();
        ActivateNewModCanvas();
        Time.timeScale = 0.0f;
    }
    public void CloseSwitchMenu()
    {
        checkingForMods = true;
        DeactivateCurrentModCanvas();
        DeactivateNewModCanvas();
        Time.timeScale = 1.0f;
    }


    private void ActivateCurrentModCanvas()
    {
        currentModCanvas.SetActive(true);
    }

    private void ActivateNewModCanvas()
    {
        NewModCanvas.SetActive(true);
    }

    public void DeactivateCurrentModCanvas()
    {
        currentModCanvas.SetActive(false);
        Time.timeScale = 1.0f;
    }
    public void DeactivateNewModCanvas()
    {
        NewModCanvas.SetActive(false);
        checkingForMods = true;
        Time.timeScale = 1.0f;
    }

    public void DeactivatePauseButton()
    {
        pauseButton.SetActive(false);
    }

    void SetModUI(int index, BulletType bulletType)
    {
        modImages[index].color = bulletType.colorOverride;
        modImages[index].sprite = bulletType.imageVisual;
    }

    void UIDisplayNewMod(BulletType bulletType)
    {
        newModImage.color = bulletType.colorOverride;
        newModImage.sprite = bulletType.imageVisual;
    }

    public void SwitchMod(int index)
    {
        //activeMods[index].gameObject;
        //StopSpecificBullet();
        if(!newMod) return;
        checkingForMods = true;
        newMod.gameObject.layer = 0;
        Destroy(sockets[index].GetChild(0).gameObject);
        activeMods[index] = newMod;

        InitBulletType(newMod, false, index);
    }

    public void CancelSwitch()
    {
        if(!newMod) return;
        Destroy(newMod.gameObject);
        checkingForMods = true;
    }

}
