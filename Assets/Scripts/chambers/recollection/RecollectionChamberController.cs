using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RecollectionChamberController : MonoBehaviour, IChamberController
{

    [SerializeField] private List<GameObject> spawns;
    [SerializeField] private List<GameObject> recollectables;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject exit;
    [SerializeField] private Animator playerAnimator;


    private List<Vector3> pos;

    private float centerDistanceW = 1f;
    private float centerDistanceH = 2.15f;
    public void OnExit()
    {
        GameManager.Instance.EnterMapScene();
    }
    private void Awake()
    {
        pos = new List<Vector3>();
        pos.Add(new Vector3(0, 0, 0));
        pos.Add(new Vector3(centerDistanceW, 0, 0));
        pos.Add(new Vector3(-centerDistanceW, 0, 0));
        pos.Add(new Vector3(0, 0, centerDistanceH));
        pos.Add(new Vector3(-centerDistanceW, 0, centerDistanceH));
        pos.Add(new Vector3(centerDistanceW, 0, centerDistanceH));
        pos.Add(new Vector3(0, 0, -centerDistanceH));
        pos.Add(new Vector3(-centerDistanceW, 0, -centerDistanceH));
        pos.Add(new Vector3(centerDistanceW, 0, -centerDistanceH));
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
                GameObject instancedObject = Instantiate(recollectables[k], pos[j], Quaternion.identity, spawns[i].transform);
            }
        }
    }
    public void StartDungeonEnterAnimation()
    {
        playerAnimator.SetFloat("Moving", 1);
        StartCoroutine(EnterDungeon());
    }
    IEnumerator EnterDungeon()
    {
        for (int i = 0; i < 1.5f / Time.fixedDeltaTime; i++)
        {
            player.transform.Translate(Vector3.forward * Time.fixedDeltaTime * 1.5f);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        playerAnimator.SetFloat("Moving", 0);
        yield return new WaitForSeconds(0.5f);
        exit.SetActive(true);
        yield return new WaitForSeconds(1);
        player.GetComponent<PlayerCombat>().enabled = true;

    }
}
