using System.Collections.Generic;
using Cooking.Recipe;
using InventorySystem;
using UnityEngine;
using PlayerStats;

using static Cooking.Recipe.RecipeManager.RecipeStyle;
using static PlayerStats.Status;

namespace Cooking
{
    [AddComponentMenu("Cooking/Cooking Device")]
    public class CookingDevice : CookingEntity
    {
        private static CookingDevice instance;
        private static ItemCode lastCookItem = ItemCode.None;
        private static bool cookingCanceled;
        private static bool resumeCooking;
        
        [SerializeField] private ItemCode defaultItemToCook = ItemCode.Jelly;
        [Space]
        [SerializeField] private GameObject fryingPan = null;
        [SerializeField] private float timeToCook = 10f;
        
        private LastCookingData cookData;
        private float minimumInteractionRange;
        
        protected override float TimeToCook { get =>timeToCook; set => timeToCook = value; }
        protected override bool ResumeAction { get => resumeCooking; set => resumeCooking = value; }
        protected override bool ActionCanceled { get => cookingCanceled; set => cookingCanceled = value; }
        protected override string AutoText => "Quick Snack";
        protected override string ManualText => "Cook Meal";
        protected override string ActionText => "Cook";
        protected override int GetNeededSkillLvl => Stats.SkillLevel(Skill.Type.Cooking);
        protected override LastCookingData CookData => cookData;

        public override void Interact()
        {
            if (!CanAct()) return;

            var displayPosition = Position;
            displayPosition.y += verticalDisplayOffset;
            RecipeManager.Style = FoodCooking;
            currentOpenCookingEntity = this;
            CookingHandler.ToggleView(Camera.main.WorldToViewportPoint(displayPosition), !cookingCanceled);

            interactionRange = Mathf.Clamp(Vector2.Distance(Position, GameLibOfMethods.player.transform.position),
                minimumInteractionRange, float.MaxValue);
        }

        protected override IEnumerator<float> StartAction(List<ItemList.ItemInfo> itemsToCook)
        {
	        cookData.itemsToCook = itemsToCook;

			onGoingAction = true;
            fryingPan.SetActive(false);
            
            CookingHandler.ForceClose();

            GameLibOfMethods.canInteract = false;
			GameLibOfMethods.cantMove = true;

			SpriteControler.Instance.FaceUP();

			Player.anim.SetBool("Cooking", true);

			float timeLapse = cookingCanceled ? cookData.timeLapse : 0;
            defaultEXP = cookingCanceled ? cookData.exp : GetItemsCookingEXP(itemsToCook, defaultEXP);
            timeToCook = GetPreparingTime(itemsToCook, defaultTimeToPrepare);

			float GetCurrentProgress() => timeLapse / timeToCook;
			int progressBarID = ProgressBar.StartTracking("Cooking", GetCurrentProgress);

			while (timeLapse < timeToCook)
			{
				timeLapse += GameTime.Time.deltaTime;
				if (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Escape) || PlayerStatsManager.statWarning.ContainsValue(true))
				{
                    resumeCooking = false;
                    cookingCanceled = true;
                    cookData.exp = defaultEXP;
                    cookData.timeLapse = timeLapse;
                    break;
				}

                if (Stats.Status(Type.Energy).CurrentAmount <= 0 ||
                    Stats.Status(Type.Health).CurrentAmount <= 0)
                {
                    break;
                }

				yield return 0f;
			}

			GameTime.Clock.ChangeSpeed(5);

			fryingPan.SetActive(true);
			yield return 0f;

			onGoingAction = false;

			ProgressBar.HideUI(progressBarID);
            PlayerAnimationHelper.ResetPlayer();

            if (Stats.Status(Type.Energy).CurrentAmount <= 0 || Stats.Status(Type.Health).CurrentAmount <= 0)
			{
				yield return 0f;
				Player.anim.SetBool("PassOut", true);
			}
			else if (resumeCooking || !cookingCanceled)
            {
	            if (!AutoCookMode)
					Stats.AddXP(Skill.Type.Cooking, defaultEXP);

				yield return 0f;

                CookingHandler.AddCookedRecipes(itemsToCook);
                Inventory.PlaceOnBag(itemsToCook);

                if (itemsToCook.Count > 0)
                    lastCookItem = itemsToCook[itemsToCook.Count - 1].itemCode;

				yield return 0f;

                cookingCanceled = false;
                resumeCooking = false;
                cookData.Reset();
            }
        }

        protected override ItemCode GetLastCookedItem()
        {
	        return lastCookItem;
        }

        protected override ItemCode DefaultItemToCook => defaultItemToCook;

        private void Awake()
        {
            instance = this;
            currentOpenCookingEntity = this; //Default cooking entity
            minimumInteractionRange = interactionRange;
        }
    }
}