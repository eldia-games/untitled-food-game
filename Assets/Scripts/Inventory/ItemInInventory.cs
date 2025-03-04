using UnityEngine;

[System.Serializable]
public struct ItemInInventory
    {
        public Items item;

        public int prefab;
        public int quantity;
        public ItemInInventory(Items item, int prefab, int quantity) : this()
        {
            this.item = item;
            this.prefab = prefab;
            this.quantity = quantity;
        }
    }