using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class InteractionMenuModel
{
    public GameObject buttonsParent, mainObj;
    public GameObject previousButton;
    public CanvasGroup canvasGroup;
    public Button[] buttons;
    public TextMeshProUGUI[] textContainers;
    public TextMeshProUGUI previousButtonTextContainer;
    EventTrigger[] buttonTriggers;

    public InteractionMenuModel(GameObject obj)
    {
        mainObj = obj;
        buttonsParent = mainObj.transform.GetChild(0).gameObject;
        previousButton = mainObj.transform.GetChild(1).gameObject;

        canvasGroup =  buttonsParent.GetComponent<CanvasGroup>();
        buttons = buttonsParent.transform.GetComponentsInChildren<Button>();
        previousButtonTextContainer = previousButton.GetComponentInChildren<TextMeshProUGUI>();
        
        canvasGroup.alpha = 0;

        textContainers = new TextMeshProUGUI[buttons.Length];
        buttonTriggers = new EventTrigger[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            textContainers[i] = buttons[i].transform.GetComponentInChildren<TextMeshProUGUI>();
            buttonTriggers[i] = buttons[i].transform.GetComponentInChildren<EventTrigger>();
        }
    }

    public IEnumerator Close()
    {
        canvasGroup.LeanAlpha(0, .2f);
        return DeactivateUI();
    }

    IEnumerator DeactivateUI()
    {
        while (canvasGroup.alpha > 0) yield return null;
        mainObj.SetActive(false);
    }
}

[System.Serializable]
public class MainMenuButtons
{
    public GameObject obj;
    [HideInInspector] public Button button;
    [HideInInspector] public MenuMouseTrigger trigger;
    [HideInInspector] public TextMeshProUGUI buttonText;
    public bool interactable
    {
        get { return button.interactable; }
        set { button.interactable = value; }
    }
    [HideInInspector] public string hoverMessage;

    public void SetupVariables()
    {
        button = obj.GetComponent<Button>();
        trigger = obj.GetComponent<MenuMouseTrigger>();
        buttonText = obj.GetComponentInChildren<TextMeshProUGUI>();
    }
}

[System.Serializable]
public class MainMenuOptions
{
    public int id;
    public string optionName;
    public bool interactable;
    [Header("If not interactable, it will display the text:")]
    public string hoverText;
}

