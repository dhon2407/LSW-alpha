using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public abstract class Storage : MonoBehaviour
    {
        public abstract void Open(ItemList data);
        public abstract void Close();
        public ItemList Itemlist => GetItemlist();
        public List<ItemSlot> ItemSlots => GetItemSlots();
        public bool IsOpen => !closed;
        public int FreeSlot => GetFreeSlot();

        protected abstract void SetupDropEvents();
        protected List<Droppable> itemCells = new List<Droppable>();
        protected Vector3 showScale;
        protected ItemList itemList;
        protected bool closed;

        public void Show()
        {
            transform.localScale = showScale;
            closed = false;
        }

        public void Hide()
        {
            transform.localScale = Vector3.zero;
            closed = true;
        }

        protected virtual void Awake()
        {
            itemCells = new List<Droppable>();
            showScale = transform.localScale;
            UpdateItemCells();
            Hide();
        }

        public bool IsFull(ItemCode currentItemCode)
        {
            foreach (var item in itemCells)
            {
                if (item.Type == Droppable.CellType.TrashBin)
                    continue;

                var itemSlot = item.GetSlot();

                if (item.Empty ||
                    (currentItemCode == itemSlot.CurrentItemCode && itemSlot.Stackable && !itemSlot.MaxStack))
                    return false;
            }

            return true;
        }
        
        public bool CanFit(ItemSlot itemSlot)
        {
            int slotToTake = 0;

            if (Contains(itemSlot.CurrentItemCode, false))
            {
                var existingSlot = SlotOf(itemSlot.CurrentItemCode, false);
                var maxStack = existingSlot.CurrentItemData.maxStack;
                if ((itemSlot.Quantity + existingSlot.Quantity) > maxStack)
                {
                    int excess = (itemSlot.Quantity + existingSlot.Quantity) - maxStack;
                    slotToTake += Mathf.CeilToInt((float)excess / maxStack);
                }
            }
            else
            {
                slotToTake++;
            }

            return slotToTake <= FreeSlot;
        }

        protected virtual void Start()
        {
            SetupDropEvents();
        }
        
        protected int GetFreeSlot()
        {
            return itemCells.Count(cell => cell.Type != Droppable.CellType.TrashBin &&
                                           cell.Empty && cell.gameObject.activeSelf);
        }

        protected void MoveToExistingItem(Droppable cell)
        {
            var itemSlot = cell.GetSlot();
            var containingCells = FindCells(itemSlot.CurrentItemCode);

            foreach (var containingCell in containingCells)
                if (!containingCell.Equals(cell))
                    itemSlot.Add(containingCell.GetSlot());
        }

        protected List<Droppable> FindCells(ItemCode itemCode)
        {
            var cellsContaining = new List<Droppable>();
            foreach (var cell in itemCells)
            {
                if (cell.Type == Droppable.CellType.TrashBin)
                    continue;

                var itemSlot = cell.GetSlot();

                if (!cell.Empty && itemCode == itemSlot.CurrentItemCode &&
                    itemSlot.Stackable && !itemSlot.MaxStack)
                    cellsContaining.Add(cell);
            }

            return cellsContaining;
        }

        protected bool Contains(ItemCode itemCode, bool includeFullStack)
        {
            foreach (var cell in itemCells)
            {
                if (cell.Type == Droppable.CellType.TrashBin)
                    continue;

                var itemSlot = cell.GetSlot();

                if (!cell.Empty && itemCode == itemSlot.CurrentItemCode)
                {
                    if (includeFullStack || itemSlot.Stackable && !itemSlot.MaxStack)
                        return true;
                }
            }

            return false;
        }

        protected ItemSlot SlotOf(ItemCode itemCode, bool includeFullStack)
        {
            foreach (var cell in itemCells)
            {
                if (cell.Type == Droppable.CellType.TrashBin)
                    continue;
                
                var itemSlot = cell.GetSlot();

                if (!cell.Empty && itemCode == itemSlot.CurrentItemCode)
                {
                    if (includeFullStack || itemSlot.Stackable && !itemSlot.MaxStack)
                        return cell.GetSlot();
                }
            }

            return null;
        }

        protected ItemSlot NextEmptyCell()
        {
            foreach (var item in itemCells)
            {
                if (item.Type == Droppable.CellType.TrashBin)
                    continue;

                if (item.Empty)
                    return Inventory.CreateSlot(item.transform);
            }

            throw new UnityException("No available slot.");
        }

        protected void SetMaxSlot(int slots)
        {
            var grid = itemCells[0].transform.parent.GetComponent<GridLayoutGroup>();
            if (grid != null)
                grid.constraintCount = Mathf.Clamp(slots, 4, 8);
            
            for (int i = 0; i < itemCells.Count; i++)
                itemCells[i].gameObject.SetActive(i < slots);
        }

        private void UpdateItemCells()
        {
            itemCells.Clear();
            foreach (var cell in GetComponentsInChildren<Droppable>())
                itemCells.Add(cell);
        }

        private ItemList GetItemlist()
        {
            itemList.Items.Clear();

            foreach (var cell in itemCells)
            {
                if (cell.Type == Droppable.CellType.TrashBin)
                    continue;

                var itemSlot = cell.GetSlot();
                itemList.Items.Add(new ItemList.ItemInfo
                {
                    itemCode = (cell.Empty) ? ItemCode.None : itemSlot.CurrentItemCode,
                    count = (cell.Empty) ? 0 : itemSlot.Quantity
                });
            }

            return itemList;
        }

        private List<ItemSlot> GetItemSlots()
        {
            List<ItemSlot> slots = new List<ItemSlot>();
            foreach (var cell in itemCells)
            {
                if (cell.Type == Droppable.CellType.TrashBin)
                    continue;

                slots.Add(cell.GetSlot());
            }

            return slots;
        }

    }
}