using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.UI;
using System.Diagnostics;

public class StartGame : MonoBehaviour
{
    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button CreditsButton;
    [SerializeField] private Button ExitButton;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        StartGameButton.onClick.AddListener(empezarPartida);
        CreditsButton.onClick.AddListener(ShowCredits);
        ExitButton.onClick.AddListener(ExitGame);


    }

    // Update is called once per frame
    void empezarPartida(){
        UnityEngine.Debug.Log("Comenzar juego");
    }
    public void ShowCredits()
    {
        UnityEngine.Debug.Log("Enseño creditos");
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        UnityEngine.Debug.Log("Salgo del juego");
#else
        // Si estamos en una build, cerramos la aplicación
        Application.Quit();
#endif
    }
}
