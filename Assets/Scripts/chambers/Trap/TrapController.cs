using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrapController : MonoBehaviour, IChamberController
{
    [SerializeField] private GameObject door;
    [SerializeField] private List<GameObject> spawns;
    [SerializeField] private List<GameObject> monsters;
    private Animator doorAnimator;
    private Animator leverAnimator;
    private List<Vector3> pos;
    private int[] monstersSpawned;
    private int enemiesLeft;
    // Start is called before the first frame update
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
        doorAnimator = door.GetComponent<Animator>();
        StartCoroutine(CloseDoor());

    }


    public void initiallise(int level)
    {
        List<int> rateList = new List<int>();
        int totalRate = 0;
        for (int i = 0; i < monsters.Count; i++)
        {

            totalRate += monsters[i].GetComponent<Spawneable>().getSpawnRate(persistence.Instance.getLevel() - 1);
            rateList.Add(totalRate);

        }
        float forceLeft = level;
        while (true)
        {
            if (monstersSpawned.Min() >= 9) break;
            int spawnPoint = (int)Mathf.Round(Random.value * (spawns.Count - 1));
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
                if (value <= forceLeft)
                {
                    forceLeft -= value;
                    // Vector3 vect = new Vector3(Random.value*2, 0, Random.value*2);

                    GameObject instancedObject = Instantiate(monsters[i], spawns[spawnPoint].transform.position + pos[monstersSpawned[spawnPoint]], Quaternion.identity);
                    enemiesLeft++;
                    monstersSpawned[spawnPoint] += 1;


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
        if (enemiesLeft==0)
        {
            StartCoroutine(OpenDoor());
        }
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
