using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChamberUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text currentBeer;
    
    public void setPlayerBeers(int beers, int maxBeers)
    {
        setBeers(beers);
        setMaxBeers(maxBeers);
    }

    public void setBeers(int beers)
    {

    }

    public void setMaxBeers(int maxBeers)
    {

    }
}
