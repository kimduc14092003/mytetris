using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject TutorialContent;
    private void FixedUpdate()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void TurnOffTutorial()
    {
        TutorialContent.SetActive(false);
    }
    public void TurnOnTutorial()
    {
        TutorialContent.SetActive(true);
    }
}
