using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using TMPro;
using UI;
using UnityEngine.UI;

namespace Cooking.Recipe
{
    public class CookingHandler : MonoBehaviour
    {
        private static CookingHandler instance;

        private const int freezerID = 0;
        private List<ItemList.ItemInfo> availableIngredients;
        private List<ItemCode> cookedRecipes;
        private List<ItemCode> seenRecipes;
        private bool isOpen = false;
        private Vector2 stoveLocation;

        [Header("Canvas Ray Caster")]
        [SerializeField] private GraphicRaycaster cookingCanvas = null;

        [Space]
        [Header("Action Buttons")]
        [SerializeField, FieldLabel("Auto / Continue")] private Button auto_continue = null;
        [SerializeField, FieldLabel("Manual / Reset")] private Button manual_reset = null;

        [Space]
        [SerializeField, FieldLabel("Manual Cooking Handler")] private ManualCookHandler manualCook = null;
        [SerializeField, FieldLabel("Auto Cooking Handler")] private AutoCookHandler autoCook = null;

        [Space]
        [SerializeField] private UIPopUp popUp = null;


        public static List<ItemList.ItemInfo> AvailableIngredients => instance.availableIngredients;
        public static List<ItemCode> CookedRecipes => instance.cookedRecipes;
        public static List<ItemCode> SeenRecipes => instance.seenRecipes;
        public static bool Ongoing => instance.isOpen;

        public static bool EnableCanvas { set => instance.cookingCanvas.enabled = value; }
        
        public static ICookMode CookMode { get; private set; }

        public static void SetCookedRecipes(List<ItemCode> savedCookedRecipes)
        {
            instance.InitializeCookedItems(savedCookedRecipes);
        }

        public static void ToggleView(Vector2 stoveLocation, bool newCook)
        {
            if (instance.isOpen)
                instance.Close();
            else
                instance.Open(stoveLocation, newCook);
        }

        public static void ForceClose()
        {
            instance.Close();
        }

        public static void AddCookedRecipes(List<ItemList.ItemInfo> itemsToCook)
        {
            foreach (var itemInfo in itemsToCook)
                instance.cookedRecipes.Add(itemInfo.itemCode);
        }

        private void UpdateIngredientSource()
        {
            availableIngredients = new List<ItemList.ItemInfo>();
            AddIngredientSource(Inventory.BagItems);
            
#if INCLUDE_FRIDGE_ITEMS
                AddIngredientSource(Inventory.GetContainerItems(freezerID));
#endif
        }

        private void AddIngredientSource(List<ItemList.ItemInfo> list)
        {
            if (list == null) return;

            foreach (var availableItem in list)
                AddToAvailableIngredients(availableItem);
        }

        private void ToggleManualCooking()
        {
            const float horizontalOffset = -300f;
            var manualCooking = manualCook.ToggleCooking();
            
            if (manualCooking && autoCook.Active)
                autoCook.Close();

            if (manualCooking)
            {
                CookMode = manualCook;
                CookMode.Refresh();
            }

            popUp.Move(manualCooking ? new Vector2(horizontalOffset, 0) : popUp.PopInPosition);
        }
        
        private void ToggleAutoCooking()
        {
            const float horizontalOffset = -300f;
            var autoCooking = autoCook.ToggleCooking();
            
            if (autoCooking && manualCook.Active)
                manualCook.Close();

            if (autoCooking)
            {
                CookMode = autoCook;
                CookMode.Refresh();
            }

            popUp.Move(autoCooking ? new Vector2(horizontalOffset, 0) : popUp.PopInPosition);
        }

        public void TakeIngredient(Item requiredItem)
        {
            for (int i = 0; i < availableIngredients.Count; i++)
            {
                if (availableIngredients[i].itemCode == requiredItem.Code)
                {
                    var ing = availableIngredients[i];
                    ing.count = Mathf.Clamp(ing.count - 1, 0, int.MaxValue);
                    availableIngredients[i] = ing;

                    TakeItemFromPlayer(requiredItem.Code);

                    return;
                }
            }
        }

        public void ReturnIngredient(Item requiredItem, int qty = 1)
        {
            var itemInfo = new ItemList.ItemInfo {itemCode = requiredItem.Code, count = qty };
            
            AddToAvailableIngredients(itemInfo);
            ReturnItemToPlayer(itemInfo);
        }

        public void Close()
        {
            EnableCanvas = false;
            manualCook.Close();
            autoCook.Close();

            popUp.Close(stoveLocation,() =>
            {
                isOpen = false;
                gameObject.SetActive(false);
            });
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            CookMode = autoCook;
        }

        private void Start()
        {
            seenRecipes = new List<ItemCode>();
            StartCoroutine(Initialize());

            GamePauseHandler.SubscribeCloseEvent(EscCloseEvent);
            GameLibOfMethods.OnPassOut.AddListener(ForceClose);
        }

        private bool EscCloseEvent()
        {
            if (!instance.isOpen) return false;
                
            instance.Close();
            return true;
        }

        private IEnumerator Initialize()
        {
            while (!Inventory.Ready || !Inventory.Initialized || cookedRecipes == null)
                yield return null;

            #region TEST: Inject initial items

#if UNITY_EDITOR
            Inventory.PlaceOnBag(new List<ItemList.ItemInfo>
            {
                new ItemList.ItemInfo {itemCode = ItemCode.Fish, count = 5},
                new ItemList.ItemInfo {itemCode = ItemCode.Bread, count = 5},
                new ItemList.ItemInfo {itemCode = ItemCode.Vegetable, count = 5},
                new ItemList.ItemInfo {itemCode = ItemCode.Meat, count = 5},
                new ItemList.ItemInfo {itemCode = ItemCode.Egg, count = 5},
                new ItemList.ItemInfo {itemCode = ItemCode.Water, count = 2},
            });
#endif

            #endregion

            UpdateIngredientSource();
            
            gameObject.SetActive(false);
        }

        private void InitializeCookedItems(List<ItemCode> savedCookedRecipes)
        {
            cookedRecipes = savedCookedRecipes ?? new List<ItemCode>();
        }

        private void Open(Vector2 location, bool newCook)
        {
            gameObject.SetActive(true);
            stoveLocation = location; 
            seenRecipes = new List<ItemCode>();

            if (newCook)
                StartCooking();
            else
                ResumeCooking();

            popUp.Open(stoveLocation, () =>
            {
                isOpen = true;
                EnableCanvas = true;
            });
        }

        private void StartCooking()
        {
            CookingEntity.ResetAction();
            UpdateIngredientSource();
            auto_continue.onClick.RemoveAllListeners();
            auto_continue.onClick.AddListener(ToggleAutoCooking);
            auto_continue.GetComponentInChildren<TextMeshProUGUI>().text = CookingEntity.AutoActionText;
            manual_reset.onClick.RemoveAllListeners();
            manual_reset.onClick.AddListener(ToggleManualCooking);
            manual_reset.GetComponentInChildren<TextMeshProUGUI>().text = CookingEntity.ManualActionText;
        }


        private void ResumeCooking()
        {
            auto_continue.onClick.RemoveAllListeners();
            auto_continue.onClick.AddListener(CookingEntity.Resume);
            auto_continue.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
            manual_reset.onClick.RemoveAllListeners();
            manual_reset.onClick.AddListener(StartNewCook);
            manual_reset.GetComponentInChildren<TextMeshProUGUI>().text = "Start again";
        }

        private void StartNewCook()
        {
            popUp.ReOpen();
            StartCooking();
        }

        private void AddToAvailableIngredients(ItemList.ItemInfo availableItem)
        {
            for (int i = 0; i < availableIngredients.Count; i++)
            {
                if (availableIngredients[i].itemCode == availableItem.itemCode)
                {
                    var ingredient = availableIngredients[i];
                    ingredient.count += availableItem.count;
                    availableIngredients[i] = ingredient;
                    return;
                }
            }

            availableIngredients.Add(availableItem);
        }

        private void TakeItemFromPlayer(ItemCode requiredItemCode)
        {
            if (Inventory.BagItems.Exists(itemInfo => itemInfo.itemCode == requiredItemCode))
            {
                Inventory.RemoveInBag(new List<ItemList.ItemInfo>
                {
                    new ItemList.ItemInfo {itemCode = requiredItemCode, count = 1}
                });
                
#if INCLUDE_FRIDGE_ITEMS             
                return;
#endif
            }

#if INCLUDE_FRIDGE_ITEMS
            
            var fridgeItems = Inventory.GetContainerItems(freezerID);

            if (fridgeItems == null) return;

            for (int i = 0; i < fridgeItems.Count; i++)
            {
                if (fridgeItems[i].itemCode == requiredItemCode)
                {
                    var itemInfo = fridgeItems[i];
                    itemInfo.count -= 1;

                    if (itemInfo.count <= 0)
                        fridgeItems.RemoveAt(i);
                    else
                        fridgeItems[i] = itemInfo;
                }
            }
#endif
        }

        private void ReturnItemToPlayer(ItemList.ItemInfo itemInfo)
        {
            if (Inventory.CanFitOnBag(new List<ItemList.ItemInfo> {itemInfo}))
            {
                Inventory.PlaceOnBag(new List<ItemList.ItemInfo> {itemInfo});
                return;
            }

            var fridgeItems = Inventory.GetContainerItems(freezerID);
            
            if (fridgeItems == null) return;
            
            for (int i = 0; i < fridgeItems.Count; i++)
            {
                if (fridgeItems[i].itemCode == itemInfo.itemCode)
                {
                    var item = fridgeItems[i];
                    item.count += itemInfo.count;
                    fridgeItems[i] = item;
                    return;
                }
            }

            fridgeItems.Add(itemInfo);
        }
    }
}
