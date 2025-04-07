using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
   // public TMP_Text roundText;
    [SerializeField] private int highestRound;
    public int startingRound;
    public int GetHighestRound()
    {
        return PlayerPrefs.GetInt("highestRound", 0);
    }

    private void Awake()
    {
       //cam = FindFirstObjectByType<Camera>();

        highestRound = GetHighestRound();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

      //  turnTime = baseTurnTime;

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cam = FindFirstObjectByType<Camera>();
        allTurrets.Clear();
        TimeResume();
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
                    //AddModToPool(newMod);
                    return;
                }

            }

        }
    }

    public void AddModToPool(GameObject newMod)
    {
        newMod.GetComponent<ModProfile>().dropped = true;
        //droppedMods.Add(newMod);
    }

  /*  public void GetStartingRound (int i)
    {
        startingRound = i;
    }*/

    public void TimePause()
    {
        Time.timeScale = 0f;
    }

    public void TimeResume()
    {
        Time.timeScale = 1.0f;
    }



}
