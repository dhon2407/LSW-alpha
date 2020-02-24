using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class HorizontalGroupUIPopUp : UIPopUp
{
    [Header("Horizontal Layout")]
    [SerializeField]
    private HorizontalLayoutGroup layoutGroup = null;
    
    protected override IEnumerator PopIn(UnityAction actionOnClose = null)
    {
        animating = true;
        mainWindow.gameObject.SetActive(true);
        mainWindow.GetComponent<CanvasGroup>().alpha = 0;

        mainWindow.localPosition = popInPosition;
        mainWindow.localScale = vectorPopInScale;

        layoutGroup.enabled = true;
        yield return new WaitForSeconds(0.01f);
        layoutGroup.enabled = false;


        popInPosition = new Vector2(mainWindow.transform.localPosition.x, popInPosition.y);

        mainWindow.localScale = vectorPopOutScale;
        mainWindow.localPosition = new Vector2(popOutPosition.x, popOutPosition.y * 2);

        LeanTween.move(mainWindow, popInPosition, duration);
        LeanTween.scale(mainWindow, vectorPopInScale, duration).setEase(popStyle);
        LeanTween.alphaCanvas(canvasGroup, 1f, duration);

        yield return new WaitForSecondsRealtime(duration);

        visible = true;
        animating = false;
    }

    protected override IEnumerator PopOut(UnityAction actionOnClose = null)
    {
        animating = true;

        LeanTween.move(mainWindow, popOutPosition, duration);
        LeanTween.scale(mainWindow, vectorPopOutScale, duration).setEase(popStyle);
        LeanTween.alphaCanvas(canvasGroup, 0f, duration);

        yield return new WaitForSeconds(duration);
        
        layoutGroup.enabled = true;
        yield return null;
        layoutGroup.enabled = false;
        
        visible = false;
        animating = false;
    }
}
