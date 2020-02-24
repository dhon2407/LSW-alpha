using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public Canvas dialogueCanvas;

    void FixedUpdate()
    {
        Vector3 CanvasPos = Camera.main.WorldToScreenPoint(this.transform.position * Time.deltaTime);
        dialogueCanvas.transform.localPosition = CanvasPos;

    }
}
