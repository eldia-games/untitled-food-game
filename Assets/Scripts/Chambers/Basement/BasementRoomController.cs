using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasementRoomController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject exit;
    [SerializeField] private Animator playerAnimator;

    public void OnExit()
    {
        GameManager.Instance.EnterLobbyScene();
    }
    private void Start()
    {
        StartDungeonEnterAnimation();
    }

    public void chooseWeapon()
    {
        UIManager.Instance.ShowWeaponsCanvas();
    }

    public void StartDungeonEnterAnimation()
    {
        playerAnimator.SetFloat("Moving", 1);
        StartCoroutine(EnterBasement());
    }
    IEnumerator EnterBasement()
    {
        float anTime=0.5f;
        float anVel = 10f;

        for (int i = 0; i < anTime / Time.fixedDeltaTime; i++)
        {
            player.transform.Translate(Vector3.forward * Time.fixedDeltaTime * anVel);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        playerAnimator.SetFloat("Moving", 0);
        yield return new WaitForSeconds(0.5f);
        exit.SetActive(true);
        yield return new WaitForSeconds(1);
        player.GetComponent<PlayerCombat>().enabled = true;

    }
}
