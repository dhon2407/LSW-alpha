using UnityEngine;

namespace InventorySystem
{
    public class ItemBuilder
    {
        public Item Build(ItemCode itemCode)
        {
            return Resources.Load<GameObject>(ItemNames.Get(itemCode)).GetComponent<Item>();
        }

        private static class ItemNames
        {
            private static readonly string Resourcelocation = "Prefabs/Items/";

            public static string Get(ItemCode code)
            {
                switch (code)
                {
                    //Add new items here
                    case ItemCode.Bread:                    return Resourcelocation + "Food/Ingredients/Bread";
                    case ItemCode.ChickenBreast:            return Resourcelocation + "Food/Chicken breast";
                    case ItemCode.CokeDiet:                 return Resourcelocation + "Food/Cork Diet";
                    case ItemCode.Coke:                     return Resourcelocation + "Food/Cork";
                    case ItemCode.CookedEgg:                return Resourcelocation + "Food/Cooked Egg";
                    case ItemCode.CookedFish:               return Resourcelocation + "Food/Cooked Fish";
                    case ItemCode.CookedSausage:            return Resourcelocation + "Food/Cooked Sausages";
                    case ItemCode.CookedToast:              return Resourcelocation + "Food/Cooked Toast";
                    case ItemCode.Croissant:                return Resourcelocation + "Food/Croissant";
                    case ItemCode.EggOnToast:               return Resourcelocation + "Food/Egg on Toast";
                    case ItemCode.EggSalad:                 return Resourcelocation + "Food/Egg Salad";
                    case ItemCode.Egg:                      return Resourcelocation + "Food/Ingredients/Eggs";
                    case ItemCode.EnglishBreakfast:         return Resourcelocation + "Food/English Breakfast";
                    case ItemCode.FishAndChips:             return Resourcelocation + "Food/Fish and Chips";
                    case ItemCode.FishOmelette:             return Resourcelocation + "Food/Fish Omelette";
                    case ItemCode.Fish:                     return Resourcelocation + "Food/Ingredients/Fish";
                    case ItemCode.Fishfingers:              return Resourcelocation + "Food/Fishfingers";
                    case ItemCode.FriedEgg:                 return Resourcelocation + "Food/Fried Egg";
                    case ItemCode.GourmetSandwich:          return Resourcelocation + "Food/Gourmet Sandwich";
                    case ItemCode.Jelly:                    return Resourcelocation + "Food/Jelly";
                    case ItemCode.MeatStew:                 return Resourcelocation + "Food/Meat Stew";
                    case ItemCode.Meat:                     return Resourcelocation + "Food/Ingredients/Meat";
                    case ItemCode.OmeletteHam:              return Resourcelocation + "Food/Omelette with Ham";
                    case ItemCode.Omelette:                 return Resourcelocation + "Food/Omelette";
                    case ItemCode.RoastDinner:              return Resourcelocation + "Food/Roast Dinner";
                    case ItemCode.SaladSupreme:             return Resourcelocation + "Food/Salad Supreme";
                    case ItemCode.Salad:                    return Resourcelocation + "Food/Salad";
                    case ItemCode.Sausages:                 return Resourcelocation + "Food/Sausages";
                    case ItemCode.SideSalad:                return Resourcelocation + "Food/Side Salad";
                    case ItemCode.Vegetable:                return Resourcelocation + "Food/Ingredients/Vegetable";
                    case ItemCode.VeggySandwich:            return Resourcelocation + "Food/Veggy Sandwich";
                    case ItemCode.Burger:                   return Resourcelocation + "Food/Burger";
                    case ItemCode.Water:                    return Resourcelocation + "Food/Water";
                    case ItemCode.Beer:                     return Resourcelocation + "Food/Beer";
                    case ItemCode.Rum:                      return Resourcelocation + "Food/Rum";
                    case ItemCode.Gin:                      return Resourcelocation + "Food/Gin";
                    case ItemCode.Vodka:                    return Resourcelocation + "Food/Vodka";
                    case ItemCode.Whiskey:                  return Resourcelocation + "Food/Whiskey";
                    case ItemCode.OnTheRocks:               return Resourcelocation + "Food/On the Rocks";
                    case ItemCode.GinTonic:                 return Resourcelocation + "Food/Gin Tonic";
                    case ItemCode.CubaLibre:                return Resourcelocation + "Food/Cuba Libre";
                    case ItemCode.MisoSoup:                 return Resourcelocation + "Food/MisoSoup";
                    case ItemCode.PoachedEgg:               return Resourcelocation + "Food/Poached Egg";
                    case ItemCode.ChickenSoup:              return Resourcelocation + "Food/Chicken Soup";
                    case ItemCode.GourmetOmelette:          return Resourcelocation + "Food/Gourmet Omelette";
                    case ItemCode.Frittata:                 return Resourcelocation + "Food/Frittata";
                    case ItemCode.BreadPudding:             return Resourcelocation + "Food/Bread Pudding";
                    case ItemCode.GourmetSoup:              return Resourcelocation + "Food/Gourmet Soup";
                    case ItemCode.EggsBenedict:             return Resourcelocation + "Food/Eggs Benedict";
                    case ItemCode.Tea:                      return Resourcelocation + "Food/Tea";

                    default:
                        throw new UnityException($"Unknown itemCode[{code}], unable to build item.");
                }
            }
        }

    }
}
