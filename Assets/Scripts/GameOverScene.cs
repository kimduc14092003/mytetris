using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScene : MonoBehaviour
{
    private Board Board;
    public TMP_Text yourScoreText;
    private void Awake()
    {
        Board = GameObject.Find("Board").GetComponent<Board>();
    }
    private void Start()
    {
        yourScoreText.text = "Your score is " + Board.score + "!";
    }
    public void OpenHomeScene()
    {
        SceneManager.LoadScene(0);
    }
   public void ReplayGame()
    {
        Board.RestartGame();
        SceneManager.UnloadScene(2);
    }
    public void MoreGame()
    {

    }
}
