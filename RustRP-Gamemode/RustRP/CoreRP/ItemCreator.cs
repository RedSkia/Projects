namespace CoreRP
{
    public struct CustomItem
    {
        internal string Shortname { get; }
        internal uint Quantity { get; }
        internal ulong SkinId { get; }
        internal CustomItem[] Mods { get; }
        internal CustomItem(string shortName, uint quantity, ulong skinid, params CustomItem[] mods)
        {
            this.Shortname = shortName;
            this.Quantity = quantity;
            this.SkinId = skinid;
            this.Mods = mods;
        }
    }
    public static class ItemCreator
    {
        public static bool GiveItem(this PlayerInventory inventory, CustomItem item, ItemContainer container = null)
        {
            if (inventory == null || item.Equals(default(CustomItem))) { return false; }
            var mainItem = ItemManager.CreateByName(item.Shortname, (int)item.Quantity, item.SkinId);
            if (mainItem == null) { return false; }
            if (item.Mods != null && item.Mods.Length > 0)
            {
                foreach (var mod in item.Mods)
                {
                    var modItem = ItemManager.CreateByName(mod.Shortname, (int)mod.Quantity, mod.SkinId);
                    if (modItem == null) { continue; }
                    modItem.MoveToContainer(mainItem.contents);
                    modItem.MarkDirty();
                }
            }
            mainItem.MarkDirty();
            inventory.GiveItem(mainItem, container);
            inventory.ServerUpdate(0);
            return true;
        }
    }
}