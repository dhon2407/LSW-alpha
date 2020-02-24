using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem
{
    public enum ItemCode
    {
        None = -1,
        //Food 0x0AXXX
        Burger              = 0x0A001,
        Bread               = 0x0A002,
        ChickenBreast       = 0x0A003,
        CokeDiet            = 0x0A004,
        Coke                = 0x0A005,
        CookedEgg           = 0x0A006,
        CookedFish          = 0x0A007,
        CookedSausage       = 0x0A008,
        CookedToast         = 0x0A009,
        Croissant           = 0x0A010,
        EggOnToast          = 0x0A011,
        EggSalad            = 0x0A012,
        Egg                 = 0x0A013,
        EnglishBreakfast    = 0x0A014,
        FishAndChips        = 0x0A015,
        FishOmelette        = 0x0A016,
        Fish                = 0x0A017,
        Fishfingers         = 0x0A018,
        FriedEgg            = 0x0A019,
        GourmetSandwich     = 0x0A020,
        Jelly               = 0x0A021,
        MeatStew            = 0x0A022,
        Meat                = 0x0A023,
        OmeletteHam         = 0x0A024,
        Omelette            = 0x0A025,
        RoastDinner         = 0x0A026,
        SaladSupreme        = 0x0A027,
        Salad               = 0x0A028,
        Sausages            = 0x0A029,
        SideSalad           = 0x0A030,
        Vegetable           = 0x0A031,
        VeggySandwich       = 0x0A032,
        MisoSoup            = 0x0A033,
        ChickenSoup         = 0x0A034,
        PoachedEgg          = 0x0A035,
        GourmetOmelette     = 0x0A036,
        Frittata            = 0x0A037,
        BreadPudding        = 0x0A038,
        GourmetSoup         = 0x0A039,
        EggsBenedict        = 0x0A040,

        //Drinks 0x0BXXX
        Water               = 0x0B001,
        Beer                = 0x0B002,
        Rum                 = 0x0B003,
        Vodka               = 0x0B004,
        Gin                 = 0x0B005,
        Whiskey             = 0x0B006,
        GinTonic            = 0x0B007,
        CubaLibre           = 0x0B008,
        OnTheRocks          = 0x0B009,
        Tea                 = 0x0B010,
    }
}
