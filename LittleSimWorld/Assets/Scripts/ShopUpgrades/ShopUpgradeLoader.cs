using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUpgradeLoader : MonoBehaviour
{
    private GameObject tempGameObjectToLoad;
    
    public GameObject LoadUpgrade(string pathOfTheGameobjectToLoad)
    {
        tempGameObjectToLoad = Resources.Load<GameObject>(pathOfTheGameobjectToLoad);

        if (tempGameObjectToLoad == null)
        {
            Debug.LogError("No upgrade found at the specified PATH: " + pathOfTheGameobjectToLoad);

            return null;
        }

        return tempGameObjectToLoad;
    }
}
