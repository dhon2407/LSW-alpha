using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Objects;

public class Interactivity : MonoBehaviour, ImenuInteraction
{
    [SerializeField] Transform centerPoint = default;
    [SerializeField] public List<MainMenuOptions> options ;
    ImenuInteraction nextLevelPrefabOptions;

    

    public void OpenMenu()
    {
        InteractionMenu.instance.ActivateInterMenu(gameObject, centerPoint, options);
    }
}


