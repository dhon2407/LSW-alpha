using System;
using InventorySystem;
using UnityEngine.Events;

namespace Cooking.Recipe
{
    public interface IRecipeSlot
    {
        IRecipeSlot SetRecipe(NewRecipe recipe);
        void CheckRequirement();
        void SetSelectAction(UnityAction action);
        Item Item { get; }
    }
    
    public enum SlotVisibility
    {
        Available,
        Locked,
        Hidden,
        SlotLock,
    }
}