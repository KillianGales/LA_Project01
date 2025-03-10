using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public List<GameObject> roundIcons;
    private int highestRound;


    void Start()
    {
        if(roundIcons.Count<1)return;
        highestRound = GameManager.Instance.GetHighestRound();
        ApplyHighestRound();
    }

    public void ApplyHighestRound()
    {
        for(int i = 0; i<(highestRound/5)+1; i++)
        {
            roundIcons[i].SetActive(true);
        }
    }

    public void LoadGameSceneWithRound()
    {
        SceneManager.LoadScene("Main");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void GetStartingRound(int round)
    {
        GameManager.Instance.startingRound = round;
    }
}
