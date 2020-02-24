using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UIPopUp : MonoBehaviour
{
    [Header("UI Transform")]
    [SerializeField]
    protected RectTransform mainWindow;

    [Space] [SerializeField] protected bool hideAtStart = true;

    [Header("Position")] [SerializeField] protected Vector2 popInPosition;
    [SerializeField] protected Vector2 popOutPosition;

    [Header("Scale")] [SerializeField] protected float popInScale;
    [SerializeField] protected float popOutScale;

    [Header("Duration Of Animation")]
    [SerializeField]
    protected float duration = 1f;

    [Header("Animation Style")]
    [SerializeField]
    protected LeanTweenType popStyle = LeanTweenType.linear;

    [SerializeField] private bool anchored = false;

    protected CanvasGroup canvasGroup;
    protected bool animating;
    protected bool visible;

    protected Vector3 vectorPopInScale => Vector3.one * popInScale;
    protected Vector3 vectorPopOutScale => Vector3.one * popOutScale;
    public bool Visible => visible;
    public Vector2 PopInPosition => popInPosition;
    public Vector2 PopOutPosition => popOutPosition;

    protected void Start()
    {
        canvasGroup = mainWindow.GetComponent<CanvasGroup>();

        CheckCG();

        if (hideAtStart)
        {
            mainWindow.localPosition = popOutPosition;
            mainWindow.localScale = vectorPopOutScale;
        }

        visible = !hideAtStart;
    }

    private void CheckCG()
    {
        if (canvasGroup != null) return;
        
        canvasGroup = mainWindow.gameObject.AddComponent<CanvasGroup>();
    }

    public void Open(UnityAction actionOnOpen = null)
    {
        if (!visible && !animating)
            StartCoroutine(PopIn(actionOnOpen));
    }

    public void Open(Vector2 popInPosition, UnityAction actionOnOpen = null)
    {
        this.popInPosition = popInPosition;
        if (!visible && !animating)
            StartCoroutine(PopIn(actionOnOpen));
    }

    public void ReOpen(UnityAction actionOnOpen = null)
    {
        StartCoroutine(PopIn(actionOnOpen));
    }

    public void Close()
    {
        Close(null);
    }

    public void Close(UnityAction actionOnClose = null)
    {
        if (visible && !animating)
            StartCoroutine(PopOut(actionOnClose));
    }

    public void Close(Vector2 popOutPosition, UnityAction actionOnClose = null)
    {
        this.popOutPosition = popOutPosition;
        if (visible && !animating)
            StartCoroutine(PopOut(actionOnClose));
    }

    public void ForceClose(UnityAction actionOnClose = null)
    {
        StopAllCoroutines();
        StartCoroutine(PopOut(actionOnClose));
    }

    public void ForceClose(Vector2 popOutPosition, UnityAction actionOnClose = null)
    {
        this.popOutPosition = popOutPosition;
        ForceClose(actionOnClose);
    }

    public void Move(Vector2 position)
    {
        if (visible && !animating)
            StartCoroutine(MoveTo(position));
    }

    private IEnumerator MoveTo(Vector2 position)
    {
        animating = true;
        LeanTween.moveLocal(mainWindow.gameObject, position, duration);

        yield return new WaitForSecondsRealtime(duration);
        animating = false;
    }

    public void ToggleState()
    {
        if (visible)
            Close();
        else
            Open();
    }

    protected virtual IEnumerator PopOut(UnityAction actionOnClose = null)
    {
        CheckCG();
        animating = true;

        LeanTween.moveLocal(mainWindow.gameObject, popOutPosition, duration);
        LeanTween.scale(mainWindow, vectorPopOutScale, duration).setEase(popStyle);
        LeanTween.alphaCanvas(canvasGroup, 0, duration);

        yield return new WaitForSecondsRealtime(duration);

        visible = false;
        animating = false;
        actionOnClose?.Invoke();
    }

    protected virtual IEnumerator PopIn(UnityAction actionOnOpen = null)
    {
        CheckCG();
        animating = true;

        mainWindow.localScale = vectorPopOutScale;
        mainWindow.localPosition = popOutPosition;

        if (anchored)
            LeanTween.move(mainWindow, popInPosition, duration);
        else
            LeanTween.moveLocal(mainWindow.gameObject, popInPosition, duration);


        LeanTween.scale(mainWindow, vectorPopInScale, duration).setEase(popStyle);
        LeanTween.alphaCanvas(canvasGroup, 1, duration);

        yield return new WaitForSecondsRealtime(duration);

        visible = true;
        animating = false;
        actionOnOpen?.Invoke();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}