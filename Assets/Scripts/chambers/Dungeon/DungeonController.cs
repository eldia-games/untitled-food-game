using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonController : MonoBehaviour,IChamberController
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject lever;
    [SerializeField] private GameObject exit;
    [SerializeField] private List<GameObject> spawns;
    [SerializeField] private List<GameObject> monsters;
    [SerializeField] private GameObject player;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private bool trap;
    private Animator doorAnimator;
    private Animator leverAnimator;
    private List<Vector3> pos;
    private int [] monstersSpawned;
    private bool leverUsed= false;
    private int enemiesLeft;

    void Awake()
    {
        monstersSpawned = new int[spawns.Count];
        for (int i = 0; i < spawns.Count; i++)
        {
            monstersSpawned[i] = 0;
        }
        pos = new List<Vector3>();
        pos.Add(new Vector3(0, 0, 0));
        pos.Add(new Vector3(1.5f, 0, 0));
        pos.Add(new Vector3(-1.5f, 0, 0));
        pos.Add(new Vector3(0, 0, 1.5f));
        pos.Add(new Vector3(-1.5f, 0, 1.5f));
        pos.Add(new Vector3(1.5f, 0, 1.5f));
        pos.Add(new Vector3(0, 0, -1.5f));
        pos.Add(new Vector3(-1.5f, 0, -1.5f));
        pos.Add(new Vector3(1.5f, 0, -1.5f));
    }

    void Start()
    {
        if (door != null)
        {
            doorAnimator = door.GetComponent<Animator>();
        }
        if (lever != null)
        {
            leverAnimator = lever.GetComponent<Animator>();
        }
       


    }
   
    public void StartDungeonEnterAnimation()
    {
        playerAnimator.SetFloat("Moving", 1);
        StartCoroutine(EnterDungeon());
    }

    // Update is called once per frame
    public void initiallise(int level)
    {
        List<int> rateList = new List<int>();
        int totalRate = 0;
        for (int i = 0; i< monsters.Count; i++)
        {

            totalRate += monsters[i].GetComponent<Spawneable>().getSpawnRate(persistence.Instance.getLevel()-1);
            rateList.Add(totalRate);

        }
        float forceLeft = level;
        while (true)
        {
            if (monstersSpawned.Min() >= 9) break;
            int spawnPoint = (int)Mathf.Round(Random.value * (spawns.Count-1));
            while (monstersSpawned[spawnPoint] >= 9)
            {
                spawnPoint = (int)Mathf.Round(Random.value * (spawns.Count - 1));
            }
           
            if (forceLeft > 0)
            {
                float random = Random.value * totalRate;
                int i = 0;
                while (rateList[i] < random)
                {
                    i++;
                }
                int value = monsters[i].GetComponent<Spawneable>().getValue();
                if (value<= forceLeft)
                {
                    forceLeft -= value;

                    GameObject instancedObject = Instantiate(monsters[i], spawns[spawnPoint].transform.position + pos[monstersSpawned[spawnPoint]], Quaternion.identity);
                    monstersSpawned[spawnPoint] += 1;
                    enemiesLeft++;


                }

            }
            else
            {
                break;
            }



        }
    }
    public void killEnemy()
    {
        enemiesLeft--;
        if (enemiesLeft == 0 && trap)
        {
            StartCoroutine(OpenDoor());
        }
    }
    public void UseLever(GameObject leverActual)
    {
        if (leverActual == lever && !leverUsed)
        {
            leverUsed = true;
            leverAnimator.SetBool("LeverLeft", !leverAnimator.GetBool("LeverLeft"));
            exit.SetActive(true);
            StartCoroutine(OpenDoor());
        }

    }
    public void OnExit()
    {
        Debug.Log("sale");
        UIManager.Instance.EnterMapScene();
    }
    IEnumerator EnterDungeon() {
        



        for (int i = 0; i < 2.2f/Time.fixedDeltaTime; i++)
        {
            player.transform.Translate(Vector3.forward * Time.fixedDeltaTime * 2);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        playerAnimator.SetFloat("Moving", 0);
        yield return new WaitForSeconds(0.5f);
        doorAnimator.SetBool("Closed", true);
        yield return new WaitForSeconds(2);
        player.GetComponent<PlayerCombat>().enabled = true;
    }
    IEnumerator OpenDoor()
    {
        yield return new WaitForSeconds(2);
        doorAnimator.SetBool("Closed", false);

    }
    IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(2);
        doorAnimator.SetBool("Closed", true);

    }
}
