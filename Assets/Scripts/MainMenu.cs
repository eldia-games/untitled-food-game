using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject credits;
    public void PlayGame()
    {
        SceneManager.LoadScene("SelectionScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
