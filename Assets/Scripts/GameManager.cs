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

    public void AddModToPool(GameObject newMod)
    {
        droppedMods.Add(newMod); 
    }

}
