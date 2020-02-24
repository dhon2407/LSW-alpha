using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using LSW.Tooltip;
using Zenject;
using System.Collections.Generic;

namespace InventorySystem
{
    public class ItemSlot : TooltipArea<ItemTooltip.Data>
    {
        private const ItemData.ItemState ActiveItem = ItemData.ItemState.Active;

        [SerializeField]
        private TMPro.TextMeshProUGUI quantity = null;
        [SerializeField]
        private Image icon = null;
        [SerializeField]
        private Image coolDownImage = null;
        [SerializeField]
        private Button button = null;

        private Item itemInside;
        private int currentQty;
        private bool onCoolDown;

        public ItemCode CurrentItemCode => itemInside.Code;
        public ItemData CurrentItemData => itemInside.Data;
        public int Quantity => currentQty;
        public int FreeQty => CurrentItemData.maxStack - currentQty;
        public Item ItemInside => itemInside;
        public bool Droppable => CurrentItemData.droppable;
        public bool Stackable => CurrentItemData.isStackable;
        public bool MaxStack => currentQty >= CurrentItemData.maxStack;
        public bool CanceledAction, InUse;
        IEnumerator FinishEating;
        protected virtual bool shouldCancel => Input.GetKeyUp(InteractionChecker.Instance.KeyToInteract) || GameLibOfMethods.passedOut;

        private void LateUpdate()
        {
            if (shouldCancel && InUse) { CancelUse(); }
        }

        public void SetSelfAction(UnityAction<ItemSlot> action)
        {
            ClearActions();
            button.onClick.AddListener(()=> action?.Invoke(this));
        }

        public void SetUseAction()
        {
            ClearActions();
            button.onClick.AddListener(UseItem);
        }

        public void ClearActions()
        {
            button.onClick.RemoveAllListeners();
        }

        public bool Same(ItemSlot item)
        {
            return (CurrentItemCode == item.CurrentItemCode);
        }

        public ItemSlot Move(ItemSlot itemSlot)
        {
            itemInside = itemSlot.itemInside;
            currentQty = itemSlot.currentQty;

            RefreshItem();

            itemSlot.Delete();

            return this;
        }

        private void RefreshItem()
        {
            name = itemInside.name;
            icon.sprite = itemInside.Data.icon;
            UpdateQty();
        }

        public void Add(ItemSlot newItemSlot)
        {
            var freeQty = CurrentItemData.maxStack - currentQty;
            
            if (freeQty == 0) return;

            Add(newItemSlot.currentQty);
            newItemSlot.Consume(freeQty);
            
            UpdateQty();
        }

        public void Add(int qty)
        {
            currentQty = Mathf.Clamp(currentQty + qty, 0, itemInside.Data.maxStack);
            UpdateQty();
        }

        public ItemSlot SetItem(Item item, int qty)
        {
            itemInside = item;
            currentQty = Mathf.Clamp(qty, 1, item.Data.maxStack);

            RefreshItem();

            return this;
        }

        public void Delete()
        {
            Destroy(gameObject);
        }

        public void SetButtonEnable(bool value)
        {
            button.enabled = value;
        }

        public void Consume(int amount)
        {
            currentQty -= amount;

			if (currentQty <= 0) { Destroy(gameObject); }
			else { UpdateQty(); }
        }

		private void UseItem()
		{
			if (!CanUseItem()) return;
			
			var itemParams = (ActiveItem) itemInside.Data;
			Player.anim.SetBool(itemParams.AnimationToPlayName, true);
			Inventory.FoodInHand = itemParams.icon;

			float t = 0;
			float GetProgress() => (t += GameTime.Time.deltaTime) / itemParams.UsageTime;
			int progressBarID = ProgressBar.StartTracking(itemParams.AnimationToPlayName, GetProgress);

            FinishEating = OnFinishEating(itemParams.UsageTime, progressBarID);
            //PlayerCommands.DelayAction(itemParams.UsageTime, OnFinishEating);
            GameLibOfMethods.cantMove = true;
            itemParams.Use();
            InUse = true;
            StartCoroutine(FinishEating);

            IEnumerator OnFinishEating(float usageTime, int progressBar)
            {
                if (!CanceledAction)
                {
                    yield return new WaitForSeconds(usageTime);
                    
                }
                itemParams.Cancel();
                Player.anim.SetBool(itemParams.AnimationToPlayName, false);
                Consume(1);
                StartCoroutine(StartCooldown(itemParams.cooldown));
                ProgressBar.HideUI(progressBarID);
                Inventory.FoodInHand = null;
                GameLibOfMethods.cantMove = false;
            }
        }

        private void CancelUse()
        {
            CanceledAction = true;
            StartCoroutine(FinishEating);
            StopCoroutine(FinishEating);
            CanceledAction = false;
            InUse = false;
        }

        private bool CanUseItem()
		{
			return !GameLibOfMethods.doingSomething &&
			       !GameLibOfMethods.cantMove &&
			       !Player.anim.GetBool("Walking") &&
			       !onCoolDown &&
			       itemInside.Data.State == ActiveItem;
		}

		public Item DropItem()
        {
            Item newItem = Instantiate(itemInside);
            newItem.SetQuantity(currentQty);
            Destroy(gameObject);

            return newItem;
        }

		IEnumerator<float> StartEating() {


			yield return 0f;
		}

		IEnumerator StartCooldown(float cooldownTime) {
			if (currentQty <= 0) { yield break; }

			onCoolDown = true;
			float timeLapse = 0;

			while (timeLapse < cooldownTime) {
				timeLapse += GameTime.Time.deltaTime;
				coolDownImage.fillAmount = Mathf.Lerp(1, 0, timeLapse / cooldownTime);
				yield return null;
			}

			coolDownImage.fillAmount = 0;
			onCoolDown = false;
		}

        private void UpdateQty()
        {
            itemInside.SetQuantity(currentQty);

            quantity.text = (currentQty > 1) ? currentQty.ToString("0") : string.Empty;
            quantity.transform.parent.localScale = (quantity.text == string.Empty) ? Vector3.zero : Vector3.one;
        }

        protected override ItemTooltip.Data TooltipData => new ItemTooltip.Data
        {
            name = itemInside.Data.name,
            description = itemInside.Data.Description,
        };
        
        public class Factory : PlaceholderFactory<ItemSlot> { }
    }
}