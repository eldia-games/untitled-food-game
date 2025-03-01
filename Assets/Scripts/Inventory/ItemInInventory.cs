[System.Serializable]
public struct ItemInInventory
    {
        public Items item;
        public int quantity;
        public ItemInInventory(Items item, int quantity) : this()
        {
            this.item = item;
            this.quantity = quantity;
        }
    }