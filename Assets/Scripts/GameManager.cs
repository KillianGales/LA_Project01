using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<Transform> allTurrets = new List<Transform>();
    public Camera cam;
   // public float turnTime, baseTurnTime;
    public bool TurnAct = false;
   // public Image timerSprite;
    public List<GameObject> droppedMods;
    [SerializeField] public List<SModRate> allMods;
    public List<ModProfile> startingMods;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

      //  turnTime = baseTurnTime;

    }

    public void AddObject(Transform obj)
    {
        if (!allTurrets.Contains(obj))
        {
            allTurrets.Add(obj);
        }
    }

    public void RemoveObject(Transform obj)
    {
        if (allTurrets.Contains(obj))
        {
            allTurrets.Remove(obj);
        }
    }

    private void Update()
    {
        /*if(turnTime > 0)
        {
            TurnAct = false;
            timerSprite.fillAmount = turnTime/baseTurnTime;
            turnTime -= Time.deltaTime;
        }
        else
        {
            TurnAct = true;
            turnTime = baseTurnTime;
        }*/
    }
    public void CheckForDrop(Transform dropPos)
    {
        if(allMods.Count>0)
        {
            float draw = Random.Range(0f,100f)/100;
            float cumul = 0f;

            foreach(SModRate mod in allMods)
            {
                cumul += mod.dropRate;
                if(draw<= cumul/*mod.dropRate*/)
                {
                    GameObject newMod = Instantiate(mod.mod, dropPos.position, dropPos.rotation, transform );
                    //allMods.Remove(mod);
                    AddModToPool(newMod);
                    return;
                }

            }

        }
    }

    public void AddModToPool(GameObject newMod)
    {
        newMod.GetComponent<ModProfile>().dropped = true;
        droppedMods.Add(newMod);
    }

    public void TimePause()
    {
        Time.timeScale = 0f;
    }

    public void TimeResume()
    {
        Time.timeScale = 1.0f;
    }

}
