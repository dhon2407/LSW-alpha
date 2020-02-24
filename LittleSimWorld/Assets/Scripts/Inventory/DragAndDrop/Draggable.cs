using System;
using Cooking.Recipe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace InventorySystem
{
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private const string canvasName = "DragAndDropCanvas";
        private const int canvasSortOrder = 999; //Top of everything??
        
        private static Droppable sourceCell;
        private static Draggable draggedItem;
        private static Canvas canvas;
        
        private GameObject icon;

        public static bool Ongoing => (draggedItem != null);
        public static Draggable Item => draggedItem;

        private void Awake()
        {
            InitializeCanvas();
        }

        private void InitializeCanvas()
        {
            if (canvas != null) return;

            GameObject canvasObj = new GameObject(canvasName);
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = canvasSortOrder;

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (CookingHandler.Ongoing) return;

            if (!GameLibOfMethods.doingSomething && GameLibOfMethods.canInteract && !GameLibOfMethods.cantMove)
            {
                sourceCell = GetCell();
                draggedItem = this;

                icon = new GameObject();

                icon.name = "Icon";
                Image myImage = GetComponentsInChildren<Image>()[1];
                icon.transform.SetParent(canvas.transform);
                myImage.raycastTarget = false;                                          // Disable icon's raycast for correct drop handling
                Image iconImage = icon.AddComponent<Image>();
                iconImage.raycastTarget = false;
                iconImage.sprite = myImage.sprite;
                RectTransform iconRect = icon.GetComponent<RectTransform>();
                // Set icon's dimensions
                RectTransform myRect = GetComponent<RectTransform>();
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.sizeDelta = new Vector2(myRect.rect.width, myRect.rect.height);
                iconImage.preserveAspect = true;
                icon.transform.localScale = Vector3.one;

                GetSlot()?.SetButtonEnable(false);

                Inventory.SFX(Inventory.Sound.StartDrag);
            }
            else
            {
                GameLibOfMethods.CreateFloatingText("You are busy, can't rearrange inventory now!", 2);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (CookingHandler.Ongoing) return;

            if (icon != null &&
                !GameLibOfMethods.doingSomething &&
                GameLibOfMethods.canInteract &&
                !GameLibOfMethods.cantMove)
            {
                icon.transform.position = Input.mousePosition;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (icon != null)
            {
                if (eventData.hovered.Count == 0)
                {
                    var slot = GetSlot();
                    if (slot != null && slot.Droppable)
                    {
                        var dropPosition = GameLibOfMethods.player.transform.position + SpriteControler.Instance.FacingVectorNorm() * .4f;
                        var itemDropOffset = .15f;
                        dropPosition.y += itemDropOffset;
                        if (CheckWall(dropPosition)) slot.DropItem().transform.position = Player.transform.position + new Vector3(0, 0, 1);
                        else slot.DropItem().transform.position = dropPosition + new Vector3(0,0,1); // henrique - adding a unit z vector to allow the interaction with the higlighter
                    }

                    Inventory.SFX(Inventory.Sound.Drop);
                }
                else
                {
                    Inventory.SFX(Inventory.Sound.Cancelled);
                }
                Clear();
            }
        }

        bool CheckWall(Vector2 pos)
        {
            var laymask = 1 << 16;
            var coll = Physics2D.OverlapCircleAll(pos, 0.1f, laymask);

            if (coll.Length == 0) return false;
            else return true;

        }

        public void Dropped()
        {
            Inventory.SFX(Inventory.Sound.Drop);
            Clear();
        }

        private void Clear()
        {
            if (icon != null)
                Destroy(icon);

            draggedItem = null;
            icon = null;
            sourceCell = null;

            GetSlot()?.SetButtonEnable(true);
        }

        public Droppable GetCell()
        {
            return GetComponentInParent<Droppable>();
        }

        public ItemSlot GetSlot()
        {
            return GetComponentInParent<ItemSlot>();
        }
    }
}
