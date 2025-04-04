using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{

    [SerializeField] private RawImage[] spriteItemBuy;
    [SerializeField] private RawImage[] spriteItemSell;
    [SerializeField] private TMP_Text[] textItemBuy;
    [SerializeField] private TMP_Text[] textItemSell;
    [SerializeField] private TMP_Text[] textQuantityBuy;
    [SerializeField] private TMP_Text[] textQuantitySell;

    private ShopController shopController;
    private List<Trade> tradesTemp;


    public void RefreshShopUI(List<Trade> tradesRecieved)
    {
        tradesTemp = tradesRecieved;
        for(int i = 0; i < tradesRecieved.Count; i++ )
        {
            try
            {
                Trade trade = tradesRecieved[i];
                Items itemIn = trade.getItemIn();
                Items itemOut = trade.getItemOut();

                int quantityIn = trade.getQuantityIn();
                int quantityOut = trade.getQuantityOut();
                spriteItemBuy[i].texture = itemIn.icon;
                spriteItemSell[i].texture = itemOut.icon;
                textItemBuy[i].text = itemIn.itemName;
                textItemSell[i].text = itemOut.itemName;
                textQuantityBuy[i].text = itemIn.quantity.ToString();
                textQuantitySell[i].text = itemOut.quantity.ToString();
            }
            catch
            {
                print("error index out of bounds: " + i);
            }
        }
    }

    public void TradeAction(int tradeIndex, bool tradeCorrect)
    {
        InventoryManager inventory = InventoryManager.Instance;
        Trade trad = tradesTemp[tradeIndex];
        if (inventory.HasItems(trad.getItemIn(), trad.getQuantityIn()))
        {
            shopController.Trade(tradeIndex);
            tradeCorrect = true;
        }
        else
        {
            tradeCorrect = false;
        }
        }

}
