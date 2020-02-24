using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnWindowResize();

public class WindowChange : UIBehaviour
{

    public static WindowChange instance = null;
    public OnWindowResize windowResizeEvent;

	protected override void Awake()
    {
        instance = this;
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        if (windowResizeEvent != null)
            windowResizeEvent();
        //Debug.Log("Window resized");
    }
}