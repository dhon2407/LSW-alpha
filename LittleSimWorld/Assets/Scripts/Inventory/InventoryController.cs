using System.Collections.Generic;
using LSW.Helpers;
using UnityEngine;
using Zenject;

namespace InventorySystem
{
    public class InventoryController : MonoBehaviour
    {
        private static InventoryController instance;

        private static ItemBag playerInventory = null;
        private static ItemContainer currentContainer = null;
        private static ItemList currentContainerData = null;
        private bool containerOpen;

        public bool ContainerOpen => containerOpen;
        public int BagFreeSlot => playerInventory.FreeSlot;
        public ItemBag PlayerInventory => playerInventory;

        [SerializeField]
        private Transform canvasTransform = null;
        [SerializeField]
        private AudioSource audioSource = null;

        [Header("SFX")]
        [SerializeField]
        private AudioClip startDragSound = null;
        [SerializeField]
        private AudioClip cancelDragSound = null;
        [SerializeField]
        private AudioClip dropSound = null;

        [Space]
        [SerializeField]
        private SpriteRenderer foodInHand = null;
        private Dictionary<int, List<ItemList.ItemInfo>> containersContents;
        
        [Inject] private ItemSlot.Factory _slotFactory = null;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            Initialize();
            Inventory.SetController(this);
        }

        public void InitializeBag(List<ItemList.ItemInfo> list)
        {
           playerInventory.InitializeItems(list);
        }

        public List<ItemList.ItemInfo> GetBagItems()
        {
            return playerInventory.Itemlist.Items;
        }

        public void InitializeContainers(Dictionary<int, List<ItemList.ItemInfo>> contents)
        {
            containersContents = contents ?? new Dictionary<int, List<ItemList.ItemInfo>>();
        }

        public Dictionary<int, List<ItemList.ItemInfo>> GetContainersContents()
        {
            return containersContents;
        }

        public void OpenContainer(ItemList list, string name)
        {
            containerOpen = true;
            currentContainer.SetName(name);

            if (!containersContents.ContainsKey(list.ID))
                containersContents[list.ID] = list.Items;
            else
                list.UpdateItems(containersContents[list.ID]);
         
            currentContainerData = list;
            currentContainer.Open(list);

            playerInventory.UpdateSlotActions(PutInCurrentContainer);

            ShowBag();
        }

        public void CloseContainer()
        {
            if (!containerOpen) return;
            
            currentContainerData = currentContainer.Itemlist;

            containersContents[currentContainerData.ID] = currentContainerData.Items;
            
            currentContainer.Close();
            currentContainerData = null;
            containerOpen = false;

            playerInventory.UpdateSlotUseActions();

            HideBag();
        }

        public void PutInBag(ItemSlot itemSlot)
        {
            if (!playerInventory.CanFit(itemSlot))
            {
                GameLibOfMethods.CreateFloatingText("Not enough space in inventory", 2);
                return;
            }
            
            playerInventory.AddItem(itemSlot);

            if (containerOpen)
                playerInventory.UpdateSlotActions(PutInCurrentContainer);
        }

        public void PutInBag(List<ItemList.ItemInfo> itemlist)
        {
            ShowBag();
            playerInventory.AddItems(itemlist);

            if (containerOpen)
                playerInventory.UpdateSlotActions(PutInCurrentContainer);
        }

        public void RemoveInBag(List<ItemList.ItemInfo> itemlist)
        {
            playerInventory.RemoveItems(itemlist);
        }

        public void PlaySFX(Inventory.Sound sound)
        {
            AudioClip clip = GetSoundClip(sound);
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        public void SetFoodInHand(Sprite sprite)
        {
            foodInHand.sprite = sprite;
        }

        public bool CanFitOnBag(List<ItemList.ItemInfo> itemlist)
        {
            return playerInventory.CanFit(itemlist);
        }

        public void ShowBag()
        {
            playerInventory.Show();
        }

        public void HideBag()
        {
            //Hiding inventory not on design
            //playerInventory.Hide();
        }

        public void SetBagItemActions(UnityEngine.Events.UnityAction<ItemSlot> action)
        {
            playerInventory.UpdateSlotActions(action);
        }

        public void ResetBagActions()
        {
            playerInventory.UpdateSlotUseActions();
        }

        public void PickupItem(Item item)
        {
            playerInventory.PickupItem(item);
        }

        private void PutInCurrentContainer(ItemSlot itemSlot)
        {
            if (!currentContainer.IsOpen) return;
            
            if (currentContainer.CanFit(itemSlot))
                currentContainer.AddItem(itemSlot);
            else
                GameLibOfMethods.CreateFloatingText($"Not enough space in {currentContainer.name}", 2);
        }

        private void Initialize()
        {
            playerInventory = Instantiate(Resources.Load<GameObject>("Inventory/PlayerInventory"), canvasTransform).
                GetComponent<ItemBag>();
            
            playerInventory.transform.SetAsFirstSibling();

            currentContainer = Instantiate(Resources.Load<GameObject>("Inventory/Container"), canvasTransform).
                GetComponent<ItemContainer>();
            
            currentContainer.transform.SetAsFirstSibling();
            
            GameLibOfMethods.OnPassOut.AddListener(CloseContainer);
        }

        private AudioClip GetSoundClip(Inventory.Sound sound)
        {
            switch (sound)
            {
                case Inventory.Sound.StartDrag:
                    return startDragSound;
                case Inventory.Sound.Drop:
                    return dropSound;
                case Inventory.Sound.Cancelled:
                    return cancelDragSound;
                default:
                    throw new UnityException("No sound clip for " + sound);
            }
        }

        public ItemSlot CreateSlot(Transform slotLocation)
        {
            var slot = _slotFactory.Create(slotLocation);
            return slot;
        }

        public ItemBag GetPlayerInventory()
        {
            return playerInventory;
        }
    }

    public static class Inventory
    {
        private static InventoryController controller;
        private static ItemBuilder itemBuilder;
        private static bool initialized;
        
        static Inventory()
        {
            itemBuilder = new ItemBuilder();
        }

        public static bool Ready => (controller != null);
        public static bool Initialized => initialized;
        public static bool ContainerOpen => (controller.ContainerOpen);
		public static ItemList currentContainer;

        public static List<ItemList.ItemInfo> BagItems => controller.GetBagItems();

        public static Dictionary<int, List<ItemList.ItemInfo>> ContainerContents => controller.GetContainersContents();

        public static Sprite FoodInHand { set => controller.SetFoodInHand(value); }
        public static int BagFreeSlot => controller.BagFreeSlot;

        public static void SetController(InventoryController inventoryController)
        {
            controller = inventoryController;
        }

        public static void Initialize()
        {
            Initialize(null, null);
        }

        public static void Initialize(List<ItemList.ItemInfo> bagContents, Dictionary<int, List<ItemList.ItemInfo>> containersContents)
        {
            controller.InitializeBag(bagContents);
            controller.InitializeContainers(containersContents);

            initialized = true;
        }

        public static void OpenCloseContainer(ItemList containerItems, string name)
        {
			if (ContainerOpen) {
				controller.CloseContainer();

				if (containerItems != currentContainer) { OpenCloseContainer(containerItems, name); }
				else { currentContainer = null; }
			}
			else {
				currentContainer = containerItems;
				controller.OpenContainer(containerItems, name);

			}
        }

        public static ItemSlot CreateSlot(Transform slotLocation)
        {
            return controller.CreateSlot(slotLocation);
        }

        public static Item CreateItem(ItemCode itemCode)
        {
            return itemBuilder.Build(itemCode);
        }

        public static Item CreateItem(ItemList.ItemInfo itemInfo)
        {
            var newItem = CreateItem(itemInfo.itemCode);
            return newItem;
        }

        public static void PlaceOnBag(ItemSlot itemSlot)
        {
            controller.PutInBag(itemSlot);
        }

        public static bool PlaceOnBag(List<ItemList.ItemInfo> itemlist)
        {
            if (!CanFitOnBag(itemlist))
                return false;
                
            controller.PutInBag(itemlist);

            return true;
        }

        public static void RemoveInBag(List<ItemList.ItemInfo> itemlist)
        {
            controller.RemoveInBag(itemlist);
        }

        public static void ShowBag()
        {
            controller.ShowBag();
        }

        public static void HideBag()
        {
            controller.HideBag();
        }

        public static bool CanFitOnBag(List<ItemList.ItemInfo> itemlist)
        {
            return controller.CanFitOnBag(itemlist);
        }

        public static void ResetBagItemActions()
        {
            controller.ResetBagActions();
        }

        public static void SetBagItemActions(UnityEngine.Events.UnityAction<ItemSlot> action)
        {
            controller.SetBagItemActions(action);
        }

        public static void SFX(Sound sound)
        {
            controller.PlaySFX(sound);
        }

        public static void PlaceOnBag(Item item)
        {
            controller.PickupItem(item);
        }

        public static List<ItemList.ItemInfo> GetContainerItems(int containerId)
        {
            var containers = controller.GetContainersContents();
            return containers.ContainsKey(containerId) ? containers[containerId] : null;
        }

        public static ItemBag GetPlayerInventory()
        {
            return controller.GetPlayerInventory();
        }

        public enum Sound
        {
            StartDrag,
            Cancelled,
            Drop,
        }
    }
}