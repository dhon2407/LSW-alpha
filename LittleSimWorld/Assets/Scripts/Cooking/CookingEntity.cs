using System.Collections.Generic;
using Cooking.Recipe;
using InventorySystem;
using UnityEngine;

using Stats = PlayerStats.Stats;
using static PlayerStats.Status;
using Objects.Functionality;
using Objects;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

namespace Cooking
{
    public abstract class CookingEntity : InteractableObject
    {
        protected static CookingEntity currentOpenCookingEntity;
        
        [SerializeField]
        protected float verticalDisplayOffset = 1480f;
        [SerializeField]
        protected float interactionRange = 0.5f;
        [SerializeField]
        protected float defaultEXP = 10f;
        [SerializeField]
        protected float defaultTimeToPrepare = 10f;
        
        [Space]
        [Header("Slot Lvl Requirements")]
        [SerializeField, Range(0, 10)] private int firstSlot = 0;
        [SerializeField, Range(0, 10)] private int secondSlot = 2;
        [SerializeField, Range(0, 10)] private int thirdSlot = 5;
        
        public abstract override void Interact();

        public override bool isValidInteractionTarget => !Open;

        public static string DeviceName => currentOpenCookingEntity.name;
        public static bool Open => currentOpenCookingEntity.onGoingAction;
        public static ItemCode LastCookedItem => currentOpenCookingEntity.GetLastCookedItem();
        public static ItemCode DefaultCookItem => currentOpenCookingEntity.DefaultItemToCook;
        public static bool IsAutoCook => currentOpenCookingEntity.AutoCookMode;
        protected bool AutoCookMode { get; set; }
        public static string AutoActionText => currentOpenCookingEntity.AutoText;
        public static string ManualActionText => currentOpenCookingEntity.ManualText;
        public static string Text => currentOpenCookingEntity.ActionText;

        public static int NeededSkillLevel => currentOpenCookingEntity.GetNeededSkillLvl;

        protected bool onGoingAction = false;
        protected abstract IEnumerator<float> StartAction(List<ItemList.ItemInfo> itemsToCook);
        protected abstract ItemCode GetLastCookedItem();
        protected abstract ItemCode DefaultItemToCook { get; }
        protected abstract float TimeToCook { get; set; }
        protected abstract bool ResumeAction { get; set; }
        protected abstract bool ActionCanceled { get; set; }
        protected abstract string AutoText { get; }
        protected abstract string ManualText { get; }
        protected abstract string ActionText { get; }
        protected abstract int GetNeededSkillLvl { get; }
        protected abstract LastCookingData CookData { get; }
        protected Vector2 Position => transform.position;
        private Transform StandArea => interactionOptions.PlayerPathfindingTarget;
       
        public static void StartAction(List<ItemList.ItemInfo> itemsToCook, bool isAuto)
        {
            currentOpenCookingEntity.AutoCookMode = isAuto;
            currentOpenCookingEntity.TryAction(itemsToCook);
        }
        
        public static void Resume()
        {
            currentOpenCookingEntity.ResumeAction = true;
            
            if (currentOpenCookingEntity.ActionCanceled)
                currentOpenCookingEntity.TryAction(currentOpenCookingEntity.CookData.itemsToCook);
        }
        
        public static void ResetAction()
        {
            currentOpenCookingEntity.ActionCanceled = false;
            currentOpenCookingEntity.CookData.Reset();
        }
        
        public static int SlotRequiredLevel(int numberOfIngredients)
        {
            Mathf.Clamp(numberOfIngredients, 1, int.MaxValue);

            switch (numberOfIngredients)
            {
                case 1: return currentOpenCookingEntity.firstSlot;
                case 2: return currentOpenCookingEntity.secondSlot;
                case 3: return currentOpenCookingEntity.thirdSlot;

                default:
                    return int.MaxValue;
            }
        }

        protected bool CanAct()
        {
            return (!onGoingAction &&
                    Stats.Status(Type.Energy).CurrentAmount > 5 &&
                    Stats.Status(Type.Health).CurrentAmount > 5);
        }

        private bool InRange() {
            var distanceAway = Vector2.SqrMagnitude(interactionOptions.PlayerPathfindingTarget.position - GameLibOfMethods.player.transform.position);
            return distanceAway <= Mathf.Pow(interactionRange, 2);
        }
        
        private void Update()
        {
            if (currentOpenCookingEntity != this) { return; }
            if (CookingHandler.Ongoing && !InRange())
                CookingHandler.ForceClose();
        }

        private void TryAction(List<ItemList.ItemInfo> itemsToCook)
        {
            if (!CanAct()) return;
            
            
            if (!Inventory.CanFitOnBag(itemsToCook))
            {
                GameLibOfMethods.CreateFloatingText("Not enough space in inventory.", 2f);
                return;
            }
                
            CookingHandler.EnableCanvas = false;
            GameLibOfMethods.doingSomething = true;
            PlayerCommands.MoveTo(StandArea.position, () => StartAction(itemsToCook).Start(MEC.Segment.LateUpdate));
        }
        
        protected float GetItemsCookingEXP(List<ItemList.ItemInfo> itemsToCook, float defaultXp)
        {
            var totalExp = 0;
            foreach (var recipeToCook in itemsToCook)
            {
                var recipe = RecipeManager.Recipes.Find(r => r.RecipeOutcome == recipeToCook.itemCode);

                if (recipe)
                    totalExp += recipe.EXPAwarded * recipeToCook.count;
            }

            return totalExp <= 0 ? defaultXp : totalExp;
        }
        
        protected float GetPreparingTime(List<ItemList.ItemInfo> itemsToCook, float defaultTime)
        {
            var timeToCook = 0f;
            foreach (var recipeToCook in itemsToCook)
            {
                var recipe = RecipeManager.Recipes.Find(r => r.RecipeOutcome == recipeToCook.itemCode);

                if (recipe)
                    timeToCook += recipe.timeToCook * recipeToCook.count;
            }

            if (currentOpenCookingEntity.AutoCookMode)
                timeToCook *= 0.55f;

            return timeToCook <= 0 ? defaultTime : timeToCook;
        }


        protected struct LastCookingData
        {
            public List<ItemList.ItemInfo> itemsToCook;
            public float timeLapse;
            public float exp;

            public void Reset()
            {
                itemsToCook = new List<ItemList.ItemInfo>();
                timeLapse = 0;
                exp = 10; //minimum default
            }
        }
        
        #region Cheat Codes

        public static float CheatEXP
        {
            set => currentOpenCookingEntity.defaultEXP = value;
        }

        public static float CheatTimeCompletion
        {
            set => currentOpenCookingEntity.TimeToCook = value;
        }

        #endregion

    }
}