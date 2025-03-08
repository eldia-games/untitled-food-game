using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RecollectionChamberController : MonoBehaviour, IChamberController
{

    [SerializeField] private List<GameObject> spawns;
    [SerializeField] private List<GameObject> recollectables;
    [SerializeField] protected GameObject player;

    private List<Vector3> pos;

    private float centerDistance = 2f;
    private void Awake()
    {
        pos = new List<Vector3>();
        pos.Add(new Vector3(0, 0, 0));
        pos.Add(new Vector3(centerDistance, 0, 0));
        pos.Add(new Vector3(-centerDistance, 0, 0));
        pos.Add(new Vector3(0, 0, centerDistance));
        pos.Add(new Vector3(-centerDistance, 0, centerDistance));
        pos.Add(new Vector3(centerDistance, 0, centerDistance));
        pos.Add(new Vector3(0, 0, -centerDistance));
        pos.Add(new Vector3(-centerDistance, 0, -centerDistance));
        pos.Add(new Vector3(centerDistance, 0, -centerDistance));
    }
    public void Start()
    {

        player.GetComponent<PlayerCombat>().enabled = true;
    }
    public void initiallise(int level)
    {
        List<int> rateList = new List<int>();
        int totalRate = 0;
        for (int i = 0; i < recollectables.Count; i++)
        {

            totalRate += recollectables[i].GetComponent<Spawneable>().getSpawnRate(persistence.Instance.getLevel() - 1);
            rateList.Add(totalRate);

        }

        for (int i = 0; i < spawns.Count; i++)
        {
            float random = Random.value * totalRate;
            int k = 0;
            while (rateList[k] < random)
            {
                k++;
            }
            for (int j = 0; j < 9; j++) {
                GameObject instancedObject = Instantiate(recollectables[k], spawns[i].transform.position + pos[j], Quaternion.identity);
            }
        }
    }
}
