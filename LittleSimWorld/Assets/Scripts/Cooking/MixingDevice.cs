using System.Collections.Generic;
using Cooking.Recipe;
using InventorySystem;
using UnityEngine;
using PlayerStats;

using static Cooking.Recipe.RecipeManager.RecipeStyle;
using static PlayerStats.Status;

namespace Cooking
{
    [AddComponentMenu("Cooking/Mixing Device")]
    public class MixingDevice : CookingEntity
    {
        private static MixingDevice instance;
        private static ItemCode lastMixItem = ItemCode.None;
        private static bool mixingCanceled;
        private static bool resumeMixing;
        
        [SerializeField] private ItemCode defaultItemToCook = ItemCode.OnTheRocks;
        [SerializeField] private float timeToMix = 10f;
        
        private LastCookingData mixData;
        private float minimumInteractionRange;
        
        protected override float TimeToCook { get =>timeToMix; set => timeToMix = value; }
        protected override bool ResumeAction { get => resumeMixing; set => resumeMixing = value; }
        protected override bool ActionCanceled { get => mixingCanceled; set => mixingCanceled = value; }
        protected override string AutoText => "Quick Mix";
        protected override string ManualText => "Manual Mix";
        protected override string ActionText => "Mix";
        protected override int GetNeededSkillLvl => Stats.SkillLevel(Skill.Type.Mixing);
        protected override LastCookingData CookData => mixData;

        public override void Interact()
        {
            if (!CanAct()) return;

            var displayPosition = Position;
            displayPosition.y += verticalDisplayOffset;
            RecipeManager.Style = DrinkMixing;
            currentOpenCookingEntity = this;
            CookingHandler.ToggleView(Camera.main.WorldToViewportPoint(displayPosition), !mixingCanceled);

            interactionRange = Mathf.Clamp(Vector2.Distance(Position, GameLibOfMethods.player.transform.position),
	            minimumInteractionRange, float.MaxValue);
        }

        protected override IEnumerator<float> StartAction(List<ItemList.ItemInfo> itemsToCook)
        {
	        mixData.itemsToCook = itemsToCook;

	        onGoingAction = true;

	        CookingHandler.ForceClose();

	        GameLibOfMethods.canInteract = false;
	        GameLibOfMethods.cantMove = true;
	        SpriteControler.Instance.FaceUP();
			SpriteControler.Instance.ChangeSortingOrder(2);
			var food = AnimatorStatesHelper.GetObjectWithName("Food").GetComponent<SpriteRenderer>();
			var occluder = AnimatorStatesHelper.GetOccluder();
			food.sortingOrder = 1;
			occluder.SetActive(false);

			Player.anim.SetBool("Mixing", true);

	        float timeLapse = mixingCanceled ? mixData.timeLapse : 0;
	        defaultEXP = mixingCanceled ? mixData.exp : GetItemsCookingEXP(itemsToCook, defaultEXP);
	        timeToMix = GetPreparingTime(itemsToCook, defaultTimeToPrepare);

			float GetCurrentProgress() => timeLapse / timeToMix;
			int progressBarID = ProgressBar.StartTracking("Mixing", GetCurrentProgress);

	        while (timeLapse < timeToMix)
	        {
		        timeLapse += GameTime.Time.deltaTime;
				food.sortingOrder = 1;
				if (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Escape) || PlayerStatsManager.statWarning.ContainsValue(true))
		        {
			        resumeMixing = false;
			        mixingCanceled = true;
			        mixData.exp = defaultEXP;
			        mixData.timeLapse = timeLapse;
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
			yield return 0f;

	        onGoingAction = false;

			ProgressBar.HideUI(progressBarID);
	        PlayerAnimationHelper.ResetPlayer();
			SpriteControler.Instance.ChangeSortingOrder(0);
			//food.sortingOrder = 1;
			occluder.SetActive(true);

			if (Stats.Status(Type.Energy).CurrentAmount <= 0 || Stats.Status(Type.Health).CurrentAmount <= 0)
	        {
		        yield return 0f;
				Player.anim.SetBool("PassOut", true);
	        }
	        else if (resumeMixing || !mixingCanceled)
	        {
		        if (!AutoCookMode)
					Stats.AddXP(Skill.Type.Mixing, defaultEXP);

		        yield return 0f;

		        CookingHandler.AddCookedRecipes(itemsToCook);
		        Inventory.PlaceOnBag(itemsToCook);

		        if (itemsToCook.Count > 0)
			        lastMixItem = itemsToCook[itemsToCook.Count - 1].itemCode;

		        yield return 0f;

		        mixingCanceled = false;
		        resumeMixing = false;
		        mixData.Reset();
	        }
        }
        

        protected override ItemCode GetLastCookedItem()
        {
            return lastMixItem;
        }

        protected override ItemCode DefaultItemToCook => defaultItemToCook;

        private void Awake()
        {
            instance = this;
            currentOpenCookingEntity = this;
            minimumInteractionRange = interactionRange;
        }
    }
}