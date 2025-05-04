using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    [Header("Shop variables")]
    private ShopController _shopController;
    private List<Trade> tradesTemp;
    private bool[] shopStatus;

    [Header("UI References")]
    public GameObject itemUIPrefab;
    public Transform contentParent;

    void Awake()
    {
        //_shopController = GetComponent<ShopController>();
        //tradesTemp = _shopController.
    }

    private void ClearExistingUI()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void RefreshShopUI(List<Trade> trades, ShopController shop)
    {
        _shopController = shop;
        shopStatus = new bool[tradesTemp.Count];
        ClearExistingUI();

        for (int i = 0; i < trades.Count; i++)
        {
            GameObject newPrefab = Instantiate(itemUIPrefab, contentParent);
            shopStatus[i] = SetupShopUI(newPrefab, trades[i], i);
        }
    }

    private bool SetupShopUI(GameObject uiElement, Trade trade, int shopStep)
    {
        bool status = false;
        InventorySafeController inventory = InventorySafeController.Instance;
        
        // Variables cantidades items
        int quantityShopSell = trade.getQuantityOut();
        int quantityInventorySell = inventory.getQuantity(trade.getItemOut());
        
        int quantityShopBuy = trade.getQuantityIn();
        int quantityInventoryBuy = inventory.getQuantity(trade.getItemOut());
        
        // Configurar nombres items
        TextMeshProUGUI nameSellText = uiElement.transform.Find("mission-backpanel/text-mask/tmp-mission-name").GetComponent<TextMeshProUGUI>();
        nameSellText.text = trade.getItemOut().name;

        TextMeshProUGUI nameBuyText = uiElement.transform.Find("mission-backpanel/text-mask/tmp-mission-name").GetComponent<TextMeshProUGUI>();
        nameBuyText.text = trade.getItemIn().name;

        // Configurar cantidad tienda 
        TextMeshProUGUI shopQuantitySell = uiElement.transform.Find("money-mask/money-quantity-box/money-quantity-text").GetComponent<TextMeshProUGUI>();
        shopQuantitySell.text = quantityShopSell.ToString();

        TextMeshProUGUI shopQuantityBuy = uiElement.transform.Find("money-mask/money-quantity-box/money-quantity-text").GetComponent<TextMeshProUGUI>();
        shopQuantityBuy.text = quantityShopBuy.ToString();

        // Configurar cantidad inventario 
        TextMeshProUGUI inventoryQuantitySell = uiElement.transform.Find("money-mask/money-quantity-box/money-quantity-text").GetComponent<TextMeshProUGUI>();
        inventoryQuantitySell.text = quantityInventorySell.ToString();

        TextMeshProUGUI inventoryQuantityBuy = uiElement.transform.Find("money-mask/money-quantity-box/money-quantity-text").GetComponent<TextMeshProUGUI>();
        inventoryQuantityBuy.text = quantityInventoryBuy.ToString();

        // Configurar los sprites
        var spriteItemSell = uiElement.transform.Find("money-mask/money-quantity-box/money-quantity-text").GetComponent<RawImage>();
        if (spriteItemSell != null)
        {
            spriteItemSell.texture = trade.getItemOut().icon;
        }

        var spriteItemBuy = uiElement.transform.Find("money-mask/money-quantity-box/money-quantity-text").GetComponent<RawImage>();
        if (spriteItemBuy != null)
        {
            spriteItemBuy.texture = trade.getItemIn().icon;
        }

        status = (quantityShopSell - quantityInventorySell) <= 0;
        
        // Configurar boton estatus
        TextMeshProUGUI buttonText = uiElement.transform.Find("complete-button/tmp-mission-complete").GetComponent<TextMeshProUGUI>();
        if (status)
            buttonText.text = "Trade";
        else
            buttonText.text = "Can't trade";

        //Configurar boton listener
        Button button = uiElement.transform.Find("complete-button").GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            ShopAction(shopStep);
        });

        return status;
    }

    public void ShopAction(int tradeIndex)
    {
        if (shopStatus[tradeIndex])
        {
            AudioManager.Instance.PlaySFXConfirmation();
            _shopController.Trade(tradeIndex);

            RefreshShopUI(_shopController.getTrades(), _shopController);
        }
        else
        {
            AudioManager.Instance.PlaySFXClose();
        }
    }

    //public void RefreshShopUIOld(List<Trade> tradesRecieved, ShopController shop)
    //{
    //    tradesTemp = tradesRecieved;
    //    shopStatus = new bool[tradesTemp.Count];
    //
    //    for (int i = 0; i < tradesRecieved.Count; i++ )
    //    {
    //        try
    //        {
    //            Trade trade = tradesRecieved[i];
    //            Items itemIn = trade.getItemIn();
    //            Items itemOut = trade.getItemOut();
    //
    //            int quantityIn = trade.getQuantityIn();
    //            int quantityOut = trade.getQuantityOut();
    //            spriteItemBuy[i].texture = itemIn.icon;
    //            spriteItemSell[i].texture = itemOut.icon;
    //            textItemBuy[i].text = itemIn.itemName;
    //            textItemSell[i].text = itemOut.itemName;
    //            textQuantityBuy[i].text = quantityIn.ToString();
    //            textQuantitySell[i].text = quantityOut.ToString();
    //            shopController = shop;
    //        }
    //        catch
    //        {
    //            print("error index out of bounds: " + i);
    //        }
    //    }
    //}

    //public bool TradeAction(int tradeIndex, bool tradeCorrect)
    //{
    //    InventoryManager inventory = InventoryManager.Instance;
    //    Trade trad = tradesTemp[tradeIndex];
    //    Debug.Log("asdfghjk00");
    //    if (inventory.HasItems(trad.getItemIn(), trad.getQuantityIn()))
    //    {
    //        shopController.Trade(tradeIndex);
    //        return  true;
    //    }
    //    else
    //    {
    //        return  false;
    //    }
    //    }

}
