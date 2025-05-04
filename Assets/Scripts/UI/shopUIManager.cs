using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    [Header("Shop variables")]
    private ShopController _shopController;
    private List<Trade> _tradesTemp;
    private bool[] shopStatus;

    [Header("UI References")]
    public GameObject itemUIPrefab;
    public Transform contentParent;

    [Header("Unity Paths")]
    private string sellItemNamePath = "sell-backpanel/text-box/text-item-sell";
    private string buyItemNamePath = "buy-backpanel/text-box/text-item-buy";

    private string sellItemShopAmountPath = "sell-backpanel/item-mask/item-minus/quantity-shop-sell";
    private string buyItemShopAmountPath = "buy-backpanel/item-mask/item-plus/quantity-shop-buy";

    private string sellItemInventoryAmountPath = "sell-backpanel/quantity-box/quantity-inventory-sell";
    private string buyItemInventoryAmountPath = "buy-backpanel/quantity-box/quantity-inventory-buy";

    private string sellItemInventorySpritePath = "sell-backpanel/item-mask/item-sell";
    private string buyItemInventorySpritePath = "buy-backpanel/item-mask/item-buy";

    private string buttonActionPath = "trade-button";
    private string buttonTextPath = "trade-button/trade-text";

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
        _tradesTemp = trades;
        shopStatus = new bool[_tradesTemp.Count];
        ClearExistingUI();

        for (int i = 0; i < _tradesTemp.Count; i++)
        {
            GameObject newPrefab = Instantiate(itemUIPrefab, contentParent);
            shopStatus[i] = SetupShopUI(newPrefab, _tradesTemp[i], i, shop);
        }
    }

    private bool SetupShopUISinComprobacion(GameObject uiElement, Trade trade, int shopStep, ShopController shop)
    {
        bool status = false;
        InventorySafeController inventory = InventorySafeController.Instance;
        
        // Variables cantidades items
        int quantityShopSell = trade.getQuantityOut();
        int quantityInventorySell = inventory.getQuantity(trade.getItemOut());
        
        int quantityShopBuy = trade.getQuantityIn();
        int quantityInventoryBuy = inventory.getQuantity(trade.getItemOut());
        
        // Configurar nombres items
        TextMeshProUGUI nameSellText = uiElement.transform.Find(sellItemNamePath).GetComponent<TextMeshProUGUI>();
        nameSellText.text = trade.getItemOut().name;

        TextMeshProUGUI nameBuyText = uiElement.transform.Find(buyItemNamePath).GetComponent<TextMeshProUGUI>();
        nameBuyText.text = trade.getItemIn().name;

        // Configurar cantidad tienda 
        TextMeshProUGUI shopQuantitySell = uiElement.transform.Find(sellItemShopAmountPath).GetComponent<TextMeshProUGUI>();
        shopQuantitySell.text = quantityShopSell.ToString();

        TextMeshProUGUI shopQuantityBuy = uiElement.transform.Find(buyItemShopAmountPath).GetComponent<TextMeshProUGUI>();
        shopQuantityBuy.text = quantityShopBuy.ToString();

        // Configurar cantidad inventario 
        TextMeshProUGUI inventoryQuantitySell = uiElement.transform.Find(sellItemInventoryAmountPath).GetComponent<TextMeshProUGUI>();
        inventoryQuantitySell.text = quantityInventorySell.ToString();

        TextMeshProUGUI inventoryQuantityBuy = uiElement.transform.Find(buyItemInventoryAmountPath).GetComponent<TextMeshProUGUI>();
        inventoryQuantityBuy.text = quantityInventoryBuy.ToString();

        // Configurar los sprites
        var spriteItemSell = uiElement.transform.Find(sellItemInventorySpritePath).GetComponent<RawImage>();
        if (spriteItemSell != null)
        {
            spriteItemSell.texture = trade.getItemOut().icon;
        }

        var spriteItemBuy = uiElement.transform.Find(buyItemInventorySpritePath).GetComponent<RawImage>();
        if (spriteItemBuy != null)
        {
            spriteItemBuy.texture = trade.getItemIn().icon;
        }

        status = (quantityShopSell - quantityInventorySell) <= 0;
        
        // Configurar boton estatus
        TextMeshProUGUI buttonText = uiElement.transform.Find(buttonTextPath).GetComponent<TextMeshProUGUI>();
        if (status)
            buttonText.text = "Trade";
        else
            buttonText.text = "Can't trade";

        //Configurar boton listener
        Button button = uiElement.transform.Find(buttonActionPath).GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            ShopAction(shopStep, shop);
        });

        return status;
    }

    private bool SetupShopUI(GameObject uiElement, Trade trade, int shopStep, ShopController shop)
    {
        bool status = false;
        InventorySafeController inventory = InventorySafeController.Instance;

        if (uiElement == null)
        {
            Debug.LogError("SetupShopUI: uiElement is null");
        }

        if (trade == null)
        {
            Debug.LogError("SetupShopUI: trade is null");
        }

        if (inventory == null)
        {
            Debug.LogError("SetupShopUI: inventory is null");
        }

        // Variables cantidades items
        int quantityShopSell = trade.getQuantityOut();
        int quantityInventorySell = inventory.getQuantity(trade.getItemOut());

        int quantityShopBuy = trade.getQuantityIn();
        int quantityInventoryBuy = inventory.getQuantity(trade.getItemOut());

        // Configurar nombres items
        TextMeshProUGUI nameSellText = uiElement.transform.Find(sellItemNamePath).GetComponent<TextMeshProUGUI>();
        if (nameSellText != null)
        {
            nameSellText.text = trade.getItemOut().name;
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find sellItemNamePath component");
        }

        TextMeshProUGUI nameBuyText = uiElement.transform.Find(buyItemNamePath).GetComponent<TextMeshProUGUI>();
        if (nameBuyText != null)
        {
            nameBuyText.text = trade.getItemIn().name;
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find buyItemNamePath component");
        }

        // Configurar cantidad tienda 
        TextMeshProUGUI shopQuantitySell = uiElement.transform.Find(sellItemShopAmountPath).GetComponent<TextMeshProUGUI>();
        if (shopQuantitySell != null)
        {
            shopQuantitySell.text = quantityShopSell.ToString();
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find sellItemShopAmountPath component");
        }

        TextMeshProUGUI shopQuantityBuy = uiElement.transform.Find(buyItemShopAmountPath).GetComponent<TextMeshProUGUI>();
        if (shopQuantityBuy != null)
        {
            shopQuantityBuy.text = quantityShopBuy.ToString();
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find buyItemShopAmountPath component");
        }

        // Configurar cantidad inventario 
        TextMeshProUGUI inventoryQuantitySell = uiElement.transform.Find(sellItemInventoryAmountPath).GetComponent<TextMeshProUGUI>();
        if (inventoryQuantitySell != null)
        {
            inventoryQuantitySell.text = quantityInventorySell.ToString();
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find sellItemInventoryAmountPath component");
        }

        TextMeshProUGUI inventoryQuantityBuy = uiElement.transform.Find(buyItemInventoryAmountPath).GetComponent<TextMeshProUGUI>();
        if (inventoryQuantityBuy != null)
        {
            inventoryQuantityBuy.text = quantityInventoryBuy.ToString();
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find buyItemInventoryAmountPath component");
        }

        // Configurar los sprites
        RawImage spriteItemSell = uiElement.transform.Find(sellItemInventorySpritePath).GetComponent<RawImage>();
        if (spriteItemSell != null && trade.getItemOut() != null)
        {
            spriteItemSell.texture = trade.getItemOut().icon;
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find sellItemInventorySpritePath component or itemOut is null");
        }

        RawImage spriteItemBuy = uiElement.transform.Find(buyItemInventorySpritePath).GetComponent<RawImage>();
        if (spriteItemBuy != null && trade.getItemIn() != null)
        {
            spriteItemBuy.texture = trade.getItemIn().icon;
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find buyItemInventorySpritePath component or itemIn is null");
        }

        status = (quantityShopSell - quantityInventorySell) <= 0;

        // Configurar boton estatus
        TextMeshProUGUI buttonText = uiElement.transform.Find(buttonTextPath).GetComponent<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = status ? "Trade" : "Can't trade";
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find buttonTextPath component");
        }

        //Configurar boton listener
        Button button = uiElement.transform.Find(buttonActionPath).GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                ShopAction(shopStep, shop);
            });
        }
        else
        {
            Debug.LogWarning("SetupShopUI: Could not find buttonActionPath component");
        }

        return status;
    }

    public void ShopAction(int tradeIndex, ShopController shop)
    {
        if (shopStatus[tradeIndex])
        {
            AudioManager.Instance.PlaySFXConfirmation();
            shop.Trade(tradeIndex);

            RefreshShopUI(_shopController.getTrades(), shop);
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
