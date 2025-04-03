using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trade
{
    private Items itemIn;
    private int quantityIn;
    private int indexLootIn;
    private Items itemOut;
    private int quantityOun;
    private int indexLootOut;


    public Trade(Items In,int quantity1,int inexIn, Items Out, int quantity2, int indexOut)
    {
        this.itemIn = In;
        this.quantityIn = quantity1;
        this.indexLootIn = inexIn;
        this.itemOut = Out;
        this.quantityOun = quantity2;
        this.indexLootOut = indexOut;
    }

    public Items getItemIn()
    {
        return this.itemIn;
    }
    public int getQuantityIn()
    {
        return this.quantityIn;
    }
    public Items getOut()
    {
        return this.itemOut;
    }
    public int getQuantityOun()
    {
        return this.quantityOun;
    }

}
