using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace InventorySystem
{
    public class Droppable : MonoBehaviour, IDropHandler
    {
        [SerializeField]
        private CellType cellType = CellType.Swap;

        public bool Empty => isEmpty();
        public CellType Type => cellType;

        public OnDropEvent onDropEvent;

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (Draggable.Ongoing)
            {
                var item = Draggable.Item;
                var sourceCell = item.GetCell();

                if (sourceCell.Equals(this))
                    item.Dropped();
                else if (Empty)
                    SetNewItem(item);
                else if (cellType == CellType.TrashBin)
                    RemoveCurrentItemWith(item);
                else
                    PlaceItem(item);
            }
        }

        private void RemoveCurrentItemWith(Draggable item)
        {
            GetSlot().Delete();
            SetNewItem(item);
        }

        public ItemSlot GetSlot()
        {
            return GetComponentInChildren<ItemSlot>();
        }

        public void PlaceItem(Draggable item)
        {
            var currentItemSlot = GetSlot();
            var newItemSlot = item.GetSlot();

            if (currentItemSlot.Same(newItemSlot) && currentItemSlot.Stackable && !currentItemSlot.MaxStack)
                currentItemSlot.Add(newItemSlot);
            else
            {
                currentItemSlot.transform.position = item.GetCell().transform.position;
                currentItemSlot.transform.SetParent(item.GetCell().transform);

                SetNewItem(item);
            }

            item.Dropped();
        }

        private void SetNewItem(Draggable item)
        {
            var slot = item.GetSlot();
            slot.transform.position = transform.position;
            slot.transform.SetParent(transform);
            
            item.Dropped();
            onDropEvent.Invoke(this);
        }

        private bool isEmpty()
        {
            var slot = GetComponentInChildren<ItemSlot>(); 
            return (slot == null || slot.CurrentItemCode == ItemCode.None);
        }

        private void Awake()
        {
            onDropEvent = new OnDropEvent();
        }

        public enum CellType
        {
            Swap,
            TrashBin,
        }

        public class OnDropEvent : UnityEvent<Droppable> { }
    }
}