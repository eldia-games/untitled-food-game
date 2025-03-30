using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecollectableMultiple : MonoBehaviour
{
    [SerializeField] private GameObject spwaneable;
    [SerializeField] private int quantity;
    [SerializeField] private List<GameObject> spawnPos;
    [SerializeField] private int itemsPerHit;
    [SerializeField] private GameObject visualDrops;
    private int quantityLeft;

    public void Start()
    {
        quantityLeft=quantity;
    }
    public void Recollect()
    {
        Debug.Log("asdfghnjmk,l");
        if (quantityLeft > 0)
        {
            int gen= quantityLeft> itemsPerHit? itemsPerHit:quantityLeft;
            quantityLeft -= gen;
            int randSpawn=(int)Mathf.Round(Random.value*spawnPos.Count)-1;
            for (int i = 0; i < gen; i++)
            {
                randSpawn=(randSpawn+1)%spawnPos.Count;
                GameObject objectCreated = Instantiate(spwaneable, spawnPos[i].transform.position, Quaternion.identity);
                ObjectDrop objectdrop = spwaneable.GetComponent<ObjectDrop>();
                objectdrop.quantity = 1;

            }
            if (quantityLeft == 0)
            {
                GetComponent<Interactable>().Deactivate();
                visualDrops.SetActive(false);
            }
        }

    }
}
