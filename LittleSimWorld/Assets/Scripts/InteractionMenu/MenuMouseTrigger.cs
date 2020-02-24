using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Objects;
using UI.Buttons;

public class MenuMouseTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static Transform activeButton;
    LSWButton button;


    public void buttonSetup(bool status)
    {
        button = gameObject.GetComponent<LSWButton>();
        if (button == null) return;
        if (status)
        {
            button.ChangeHighlight(button._pressedColor);
            button.enabled = false;
        }
        else
        {
            button.enabled = true;
            button.ChangeHighlight(button._normalColor);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        activeButton = gameObject.transform;
        InteractionMenu.instance.ShowUINotAvailable(gameObject, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        activeButton = null;
        InteractionMenu.instance.ShowUINotAvailable(gameObject, false);
    }



}

