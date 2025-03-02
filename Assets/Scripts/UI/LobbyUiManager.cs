using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerLobby : MonoBehaviour
{
    [SerializeField] private Canvas WeaponSelect;
    [SerializeField] private Canvas Achievements;
    [SerializeField] private Canvas MisionCanvas;
    [SerializeField] private Canvas UpgradesCanvas;
    [SerializeField] private Canvas HelpCanvas;
    public GameObject Panel;

    private void Awake()
    {
        WeaponSelect.gameObject.SetActive(false);
        Achievements.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false);
    }

    public void ShowWeaponSelect()
    {
        WeaponSelect.gameObject.SetActive(true);
        Achievements.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false);
        UnityEngine.Debug.Log("Muestro Seleccion Armas");
    }

    public void ShowMisionCanvas()
    {
        UnityEngine.Debug.Log("Muestro misiones");
        WeaponSelect.gameObject.SetActive(false);
        Achievements.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(true);
        UpgradesCanvas.gameObject.SetActive(false);
    }
    public void ShowUpgradeCanvas()
    {
        UnityEngine.Debug.Log("Muestro Mejoras");
        WeaponSelect.gameObject.SetActive(false);
        Achievements.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(true);

    }
    public void ShowAchievementsCanvas()
    {
        UnityEngine.Debug.Log("Muestro logros");
        WeaponSelect.gameObject.SetActive(false);
        Achievements.gameObject.SetActive(true);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false);

    }
    public void ShowHelpCanvas()
    {
        UnityEngine.Debug.Log("Muestro ayuda");
        WeaponSelect.gameObject.SetActive(false);
        Achievements.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false);
        HelpCanvas.gameObject.SetActive(true);
    }

    public void CloseCanvas()
    {
        UnityEngine.Debug.Log("Cierro todos los canvas");
        WeaponSelect.gameObject.SetActive(false);
        Achievements.gameObject.SetActive(false);
        MisionCanvas.gameObject.SetActive(false);
        UpgradesCanvas.gameObject.SetActive(false); 
        HelpCanvas.gameObject.SetActive(false);
        Panel.gameObject.SetActive(true);
    }

    public void EnterMap()
    {
        SceneManager.LoadScene("Map");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        UnityEngine.Debug.Log("Salgo del juego");
#else
        // Si estamos en una build, cerramos la aplicación
        OnExitGame?.Invoke();
        Application.Quit();
#endif
    }

}
