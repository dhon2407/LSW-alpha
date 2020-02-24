using System.Collections.Generic;
using System.Linq;
using Cooking;
using Items.Shops;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    public class ItemBag : Storage
    {
#if ITEM_INVENTORY_HIDE_SUPPORTED
        [SerializeField]
        private KeyCode functionKey = KeyCode.I;
#endif
        private UnityAction<ItemSlot> openBagSlotAction;

        public override void Close() { }

        public override void Open(ItemList data)
        {
            if (data == null)
                return;

            itemList = data;

            int currentSlot = 0;
            foreach (var itemData in data.Items)
            {
                if (itemData.itemCode != ItemCode.None)
                    Inventory.CreateSlot(itemCells[currentSlot].transform).
                        SetItem(Inventory.CreateItem(itemData), itemData.count).
                        SetUseAction();

                currentSlot++;
            }
        }

        public void InitializeItems(List<ItemList.ItemInfo> list)
        {
            if (list == null)
                return;

            itemList.UpdateItems(list);

            Open(itemList);
        }

        public bool AddItem(ItemSlot itemSlot)
        {
            if (IsFull(itemSlot.CurrentItemCode))
                return false;

            if (Contains(itemSlot.CurrentItemCode,false))
                SlotOf(itemSlot.CurrentItemCode,false).Add(itemSlot);
            else
                NextEmptyCell().Move(itemSlot).SetUseAction();

            Inventory.SFX(Inventory.Sound.Drop);

            return true;
        }

        public void AddItem(ItemList.ItemInfo itemInfo)
        {
            int maxStack = Inventory.CreateItem(itemInfo.itemCode).Data.maxStack;
            int remainingCount = itemInfo.count;

            while (remainingCount > 0)
            {
                int bundleCount = Mathf.Clamp(remainingCount, 0, maxStack);
                var newItem = NextEmptyCell().SetItem(Inventory.CreateItem(itemInfo.itemCode), bundleCount);
                
                if (openBagSlotAction != null)
                    newItem.SetSelfAction(openBagSlotAction);
                else
                    newItem.SetUseAction();

                remainingCount -= bundleCount;
            }
        }

        public bool CanFit(List<ItemList.ItemInfo> itemlist)
        {
            int slotToTake = 0;

            foreach (var item in itemlist)
            {
                int maxStack = Inventory.CreateItem(item.itemCode).Data.maxStack;
                
                if (Contains(item.itemCode, false))
                {
                    var existingSlot = SlotOf(item.itemCode, false);
                    if ((item.count + existingSlot.Quantity) > maxStack)
                    {
                        int excess = (item.count + existingSlot.Quantity) - maxStack;
                        slotToTake += Mathf.CeilToInt((float)excess / maxStack);
                    }
                }
                else
                {
                    slotToTake++;
                    if (item.count > maxStack)
                    {
                        int excess = item.count - maxStack;
                        slotToTake += Mathf.CeilToInt((float)excess / maxStack);    
                    }
                    
                }
            }

            return slotToTake <= FreeSlot;
        }

        public void AddItems(List<ItemList.ItemInfo> itemlist)
        {
            foreach (var item in itemlist)
            {
                if (Contains(item.itemCode, false))
                {
                    var existingSlot = SlotOf(item.itemCode,false);
                    
                    int excessQty = (existingSlot.Quantity + item.count) - existingSlot.CurrentItemData.maxStack;
                    if (excessQty > 0)
                        AddItem(new ItemList.ItemInfo
                        {
                            itemCode = item.itemCode,
                            count = excessQty,
                        });
                    
                    SlotOf(item.itemCode,false).Add(item.count);
                }
                else
                    AddItem(item);
            }
        }

        public void PickupItem(Item item)
        {
            if (!IsFull(item.Code))
            {
                var slotWithItem = SlotOf(item.Code, false);
                if (slotWithItem != null && slotWithItem.FreeQty >= item.Count)
                {
                    slotWithItem.Add(item.Count);
                    Destroy(item.gameObject);
                    return;
                }
                
                if (FreeSlot != 0)
                {
                    AddItem(new ItemList.ItemInfo
                    {
                        itemCode = item.Code,
                        count = item.Count
                    });

                    Destroy(item.gameObject);
                    return;
                }
            }
            
            GameLibOfMethods.CreateFloatingText("Not enough space in inventory", 2);
                
        }

        public void RemoveItems(List<ItemList.ItemInfo> itemlist)
        {
            foreach (var item in itemlist)
            {
                if (Contains(item.itemCode, true))
                    SlotOf(item.itemCode,true).Consume(item.count);
            }
        }

        public void UpdateSlotActions(UnityAction<ItemSlot> slotAction)
        {
            openBagSlotAction = slotAction;
            foreach (var cell in itemCells)
                if (!cell.Empty)
                    cell.GetSlot().SetSelfAction(slotAction);
        }

        public void UpdateSlotUseActions()
        {
            openBagSlotAction = null;
            foreach (var cell in itemCells)
                if (!cell.Empty)
                    cell.GetSlot().SetUseAction();
        }

        protected override void SetupDropEvents()
        {
            foreach (var cell in itemCells)
            {
                cell.onDropEvent.AddListener(EnableAction);
                cell.onDropEvent.AddListener(MoveToExistingItem);
            }
        }

        private void EnableAction(Droppable cell)
        {
            var cellSlot = cell.GetSlot();
            if (cell.Type == Droppable.CellType.TrashBin)
                cellSlot.ClearActions();
            else if (Inventory.ContainerOpen || BaseShopUI.isOpen || CookingEntity.Open)
                cellSlot.SetSelfAction(openBagSlotAction);
            else
                cellSlot.SetUseAction();
        }

        protected override void Start()
        {
            base.Start();
            Show();
        }

        protected override void Awake()
        {
            base.Awake();
            itemList = GetComponent<ItemList>();
            Show();
        }

#if ITEM_INVENTORY_HIDE_SUPPORTED
        private void Update()
        {
            if (!Inventory.Ready) return;

            if (Input.GetKeyDown(functionKey))
            {
                if (closed)
                    Show();
                else
                    Hide();
            }
        }
#endif
    }
}