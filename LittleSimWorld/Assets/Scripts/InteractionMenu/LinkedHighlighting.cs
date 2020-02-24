using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightPlus2D;

public class LinkedHighlighting : MonoBehaviour
{
    [SerializeField] HighlightManager2D HLManager = default;
    [SerializeField] Transform parentObject1 = default, parentObject2 = default;
    Transform linkedObj1, linkedObj2;
    HighlightEffect2D effect1, effect2;
    bool highlightCurrent = false, highlightTrue = false;
    
    void Start()
    {
        linkedObj1 = parentObject1.GetChild(0);
        effect1 = parentObject1.GetComponent<HighlightEffect2D>();
        linkedObj2 = parentObject2.GetChild(0);
        effect2 = parentObject2.GetComponent<HighlightEffect2D>();
    }

    void Update()
    {
        if (HLManager.currentObject == linkedObj1 || HLManager.currentObject == linkedObj2) highlightCurrent = true;
        else highlightCurrent = false;

        if (highlightCurrent == highlightTrue) return;

        if (!highlightCurrent) { effect1.SetHighlighted(false); effect2.SetHighlighted(false); highlightTrue = highlightCurrent;  return; }

        SetupObjects();

        if (HLManager.currentObject == linkedObj1) { effect2.SetTarget(linkedObj2); effect2.SetHighlighted(true); }
        else if (HLManager.currentObject == linkedObj2) { effect1.SetTarget(linkedObj1); effect1.SetHighlighted(true); }

        highlightTrue = highlightCurrent;
    }

    
    public void SetupObjects()
    {
        if (linkedObj1 == null)
        {
            linkedObj1 = parentObject1.GetChild(0);
        }
        if (linkedObj2 == null)
        {
            linkedObj2 = parentObject2.GetChild(0);
        }
    }
}
