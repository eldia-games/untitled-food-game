using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recollectable : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject spwaneable;
    public void Recollect()
    {
        GameObject objectCreated = Instantiate(spwaneable, transform.position+Vector3.up*0.8f, Quaternion.identity);
        ObjectDrop objectdrop = spwaneable.GetComponent<ObjectDrop>();
        objectdrop.quantity = 1;
        Destroy(gameObject);

    }
}
