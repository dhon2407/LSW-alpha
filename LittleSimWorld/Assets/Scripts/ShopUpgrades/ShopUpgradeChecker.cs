using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUpgradeChecker : MonoBehaviour
{
    public bool LoadedUpgradeIsTheCorrectUpgrade(string upgradeName, int id)
    {
        if (int.Parse(upgradeName) == id)
        {
            return true;
        }

        return false;
    }
}
