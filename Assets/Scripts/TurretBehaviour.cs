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


    void Awake()
    {
        Canon = transform.Find("Canon");
        Launchpad = Canon.transform.Find("LaunchPad");
        turretBase = transform.Find("Base");

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

            //ModProfile curModData = curMod.GetComponent<ModProfile>();

            SetModUI(index, curModData.bulletType);

            if(shootRoutines[index] != null) StopCoroutine(shootRoutines[index]);

            switch(index)
            {
                case 0 :
                    //StopCoroutine(shootRoutines[0]);
                    shootRoutines[0] = StartCoroutine(EAutoShoot00(curModData));
                    break;
                case 1 :
                    //StopCoroutine(shootRoutines[1]);
                    shootRoutines[1] = StartCoroutine(EAutoShoot01(curModData));
                    break;
                case 2 :
                    //StopCoroutine(shootRoutines[2]);
                    shootRoutines[2] = StartCoroutine(EAutoShoot02(curModData));
                    break;
            }

            newMod = null;
            /*for(int i = 0 ; i < shootRoutines.Count; i++)
            {
                Debug.Log(i + "th routine is " + shootRoutines[i]);
            }*/
            /*Coroutine newCoroutine =*/
            /*Dictionary sys for coroutines activeModRoutine[curModData] = newCoroutine;*/
/*
            switch (curModData.type)
            {
                case EmodTypes.CanonBall:

                    /*activeModRoutine[index] = Coroutine newCoroutine =  StartCoroutine(EAutoShoot(curModData));
                    Debug.Log(name + "is set to fire CanonBall");
                    break;

                case EmodTypes.FastBullet:

                    /*activeModRoutine[index] = Coroutine newCoroutine =StartCoroutine(EAutoShoot(curModData));
                    Debug.Log(name + "is set to fire FastBullet");
                    break;
            }
*/
        }
    }
/*Dictionary sys for coroutines
    void StopSpecificBullet(ModProfile modType)
    {
        if (activeModRoutine.ContainsKey(modType))
        {
            StopCoroutine(activeModRoutine[modType]);
            activeModRoutine.Remove(modType);
        }
    }*/
    /*
    void ToggleBulletType(ModProfile modType)
    {
        if (activeMods.Contains(modType))
        {
            activeModRoutine.Remove(modType);
            StopSpecificBullet(modType);
            Debug.Log($"Stopped firing {modType}");
        }
        else
        {
            activeMods.Add(modType);
            Debug.Log($"Started firing {modType}");
        }

    }*/
    

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


        if(GameManager.Instance.droppedMods.Count > 0 && checkingForMods)
        {
            CheckForMod();
        }

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
                if(obj == GameManager.Instance.droppedMods[i])
                {
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
                }
            }

        }
    }

    private IEnumerator EmptyRoutine()
    {
        yield break;
    }



    private void CanonLookat()
    {
        WorldMousePos = InputManager.GetWorldMousePosition();

        direction = WorldMousePos - Canon.position;
        direction.y = 0;

        targetRotation = Quaternion.LookRotation(direction);
        Canon.rotation = Quaternion.Slerp(Canon.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

/// <summary>
/// //Start of Coroutines section//////
/// </summary>
//</param>

    private IEnumerator EAutoShoot00(ModProfile curModData)
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
            StartCoroutine(PoolReset(activeBullet, Ibullet.lifeSpan));

            //AutoShoot();
            yield return new WaitForSeconds(fireRate);
        }  
    }

    private IEnumerator EAutoShoot01(ModProfile curModData)
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
            StartCoroutine(PoolReset(activeBullet, Ibullet.lifeSpan));

            //AutoShoot();
            yield return new WaitForSeconds(fireRate);
        }  
    }

    private IEnumerator EAutoShoot02(ModProfile curModData)
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
            StartCoroutine(PoolReset(activeBullet, Ibullet.lifeSpan));

            //AutoShoot();
            yield return new WaitForSeconds(fireRate);
        }  
    }

/// <summary>
/// //End of Coroutines section//////
/// </summary>
//</param>

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
    private IEnumerator PoolReset(GameObject currentBullet, float bulletLifeSpan)
    {
        yield return new WaitForSeconds(bulletLifeSpan);
        bulletPool.ReturnObject(currentBullet);
    }

 /*   void TurnAct()
    {
        AutoShoot();
    }
*/
    void ModPickup(ModProfile mod)
    {
        mod.gameObject.layer = 0;
        activeMods.Add(mod);

        //int index = activeMods.Count;

        /* Other non working logic to check an empty socket - Fix by putting an empty gameobject in empty sockets
        for(int i = 0; i < 2; i++)
        {
            if(activeMods[i] == null)
            {
                index = i;
            }

        }*/

        //
        InitBulletType(mod, false, activeMods.Count-1);
        Debug.Log("Starting the " + (activeMods.Count-1) + "th coroutine");
        //InitBulletType(activeMods.Count-1, false);
        
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
