using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;
using Stats = PlayerStats.Stats;
using static PlayerStats.Status.Type;
using PlayerStatus = System.Collections.Generic.Dictionary<PlayerStats.Status.Type, PlayerStats.Status>;

public class WorkingState : StateMachineBehaviour
{
    public float BladderReduce = 300;
    public bool UsingToilet;
    public float EatingTime = 0;
    public bool Ate, Drank;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MEC.Timing.RunCoroutine(JobCar.Instance.DoingJob());
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Stats.Status(Bladder).CurrentAmount < (Stats.Status(Bladder).MaxAmount * .2) || UsingToilet)
        {
            UsingToilet = true;
            SimulateToilet();
        }
        if (Stats.Status(Thirst).CurrentAmount < (Stats.Status(Thirst).MaxAmount * .2) && !UsingToilet && !Drank)
        {
            SimulateDrinking();
        }

        if (Stats.Status(Hunger).CurrentAmount < (Stats.Status(Hunger).MaxAmount * .2) && !UsingToilet && !Ate)
        {
            SimulateEating();
        }
    }



    private void SimulateToilet()
    {
        float t = GameTime.Clock.InGameHoursSinceLastFrame;
        Stats.Status(Bladder).Add(t * BladderReduce);
        if (Stats.Status(Bladder).CurrentAmount == Stats.Status(Bladder).MaxAmount)
        {
            UsingToilet = false;
        }
    }

    private void SimulateDrinking()
    {
        if (!ConsumeItem("Water", 2))
            BuyConsumable();

        Drank = true;
    }

    private void SimulateEating()
    {
        if (!ConsumeItem("", 2, true))
            BuyConsumable(true);

        Ate = true;
    }

    private void BuyConsumable(bool Food = false)
    {
        var virtualShopItems = GameObject.Find("Career Scripts").GetComponent<Items.Shops.GeneralStore>().Items;
        List<InventorySystem.ItemList.ItemInfo> shopList = new List<InventorySystem.ItemList.ItemInfo>();
        InventorySystem.ItemList.ItemInfo itemToBuy = new InventorySystem.ItemList.ItemInfo();
        float price = -1;
        foreach (var Item in virtualShopItems)
        {
            if (Item.Name == "Water" && !Food)
            {
                price = Item.price * 2;
            }
            else if (Item.Name != "Water" && Food)
            {
                if (Item.ItemType == Items.ItemType.Food)
                    price = ItemIngredientPrice(Item);
            }
            if (Stats.Money >= price && price > 0)
            {
                Stats.RemoveMoney(price);
                itemToBuy.itemCode = Item.code;
                itemToBuy.count = 1;
                shopList.Add(itemToBuy);
                InventorySystem.Inventory.PlaceOnBag(shopList);
                ConsumeItem(Item.name, 1);
                return;
            }
            else if (Item.Name == "Water" && !Food)
                return;
        }
    }

    private float ItemIngredientPrice(InventorySystem.ItemData item)
    {
        var virtualShopItems = GameObject.Find("Career Scripts").GetComponent<Items.Shops.GeneralStore>().Items;
        var recipes = Cooking.Recipe.RecipeManager.Recipes;
        foreach (var recipe in recipes)
        {
            if (recipe.RecipeOutcome == item.code)
            {
                foreach (var Item in virtualShopItems)
                {
                    if (Item == item)
                    {
                        foreach (var requiredItem in virtualShopItems)
                        {
                            if (requiredItem.code == recipe.itemsRequired[0])
                                return item.price * 2;
                        }
                        return -1;
                    }
                }
            }
        }
        return -1;
    }

    private bool ConsumeItem(string itemName, int quantity, bool food = false)
    {
        var playerInventory = InventorySystem.Inventory.GetPlayerInventory();
        foreach (var item in playerInventory.ItemSlots)
        {
            if (item != null)
            {
                InventorySystem.ActiveItem consumable;
                if (item.name == itemName && item.Quantity >= quantity)
                {
                    consumable = (InventorySystem.ActiveItem)item.ItemInside.Data;
                    consumable.Use();
                    item.Consume(quantity);
                    return true;
                }
                else if (food)
                {
                    if (item.ItemInside.Data.ItemType == Items.ItemType.Food && item.ItemInside.Data.kind != InventorySystem.ItemData.ItemKind.Ingredient)
                    {
                        consumable = (InventorySystem.ActiveItem)item.ItemInside.Data;
                        consumable.Use();
                        item.Consume(quantity);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Drank = false;
        Ate = false;
        UsingToilet = false;
    }

    //To Be kept in case the player will be able to cook at work

    //private void SimulateCooking(int quantity)
    //{
    //    var playerInventory = InventorySystem.Inventory.GetPlayerInventory();
    //    var recipes = Cooking.Recipe.RecipeManager.Recipes;
    //    List<InventorySystem.ItemList.ItemInfo> itemList = new List<InventorySystem.ItemList.ItemInfo>();
    //    InventorySystem.ItemList.ItemInfo itemToCook = new InventorySystem.ItemList.ItemInfo();

    //    foreach (var recipe in recipes)
    //    {
    //        if (recipe.itemsRequired.Count == 1)
    //        {
    //            foreach (var item in playerInventory.ItemSlots)
    //                if (item != null)
    //                {
    //                    if (recipe.itemsRequired[0] == item.ItemInside.Code && item.Quantity >= quantity)
    //                    {
    //                        item.Consume(quantity);
    //                        itemToCook.itemCode = recipe.RecipeOutcome;
    //                        itemToCook.count = 1;
    //                        itemList.Add(itemToCook);
    //                        InventorySystem.Inventory.PlaceOnBag(itemList);
    //                        ConsumeItem(Enum.GetName(typeof(InventorySystem.ItemCode), itemToCook.itemCode), 1);
    //                    }
    //                }
    //        }
    //    }
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
