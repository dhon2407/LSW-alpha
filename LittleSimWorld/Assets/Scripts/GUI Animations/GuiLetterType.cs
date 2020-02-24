using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuiLetterType : MonoBehaviour
{

    public TextMeshProUGUI text;
    [Header("Transition Type")]
    public LeanTweenType type;
    public float typingDelay;
    public float startDelay;

    public bool loop;
    


    private void Start()
    {
        LetterType();
    }
    void LetterType()
    {
        // Write in paragraph text
        string origText = text.text;
        text.text = "";
        if (loop)
        {
            LeanTween.value(gameObject, 0, (float)origText.Length, typingDelay).setEase(type).setOnUpdate((float val) =>
            {
                text.text = origText.Substring(0, Mathf.RoundToInt(val));
            }).setLoopClamp().setDelay(startDelay);
        }
        else
        {
            LeanTween.value(gameObject, 0, (float)origText.Length, typingDelay).setEase(type).setOnUpdate((float val) =>
            {
                text.text = origText.Substring(0, Mathf.RoundToInt(val));
            });
        }
    }
}
