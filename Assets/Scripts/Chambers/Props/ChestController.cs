using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChestController : MonoBehaviour
{

    [SerializeField] private List<GameObject> loot;
    private  GameObject chosenItem;
    [SerializeField] private UnityEvent<GameObject> openFunction;
    // Start is called before the first frame update
    void Start()
    {

        int lootIndex = (int)(Mathf.Floor(Random.value * loot.Count));
        chosenItem = loot[lootIndex];



    }

    public void OnOpen()
    {
s        if (chosenItem != null)
        {
            GameObject objectCreated = Instantiate(chosenItem, transform.position, Quaternion.identity);
            ObjectDrop objectdrop;
            GetComponent<Animator>().SetBool("isClosed", false);
            if ((objectdrop = chosenItem.GetComponent<ObjectDrop>()) != null)
            {
                objectdrop.quantity = (int)Mathf.Ceil((float)(persistence.Instance.getLevel()) / objectCreated.GetComponent<ObjectDrop>().getValue());
            }
            GetComponent<Interactable>().Deactivate();
            if (openFunction != null)
            {
                openFunction.Invoke(gameObject);
            }
        }
    }

   
   
}
