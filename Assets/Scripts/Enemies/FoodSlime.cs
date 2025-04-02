using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FoodSlime : MonoBehaviour
{
    [Header("Stats generales")]
    // Lista de todas las comidas en las que se puede transformar el slime
    // Se puede añadir más comidas a la lista en el inspector
    public List<GameObject> listaComida = new List<GameObject>();
    

    // Start is called before the first frame update
    void Start()
    {
        // Inicializa el mesh del slime con una comida aleatoria de la lista
        int randomIndex = Random.Range(0, listaComida.Count);
        GameObject randomMesh = listaComida[randomIndex];
        // Activar el mesh del slime
        randomMesh.SetActive(true);
        // Desactivar el resto de los meshes
        for (int i = 0; i < listaComida.Count; i++)
        {
            if (i != randomIndex)
            {
                listaComida[i].SetActive(false);
            }
        }
        // Cambiar el nombre del slime al de la comida
        string nombreComida = randomMesh.name;
        gameObject.name = nombreComida;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
