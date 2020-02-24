using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.Events;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(ScrollRect))]
public class ScrollSnap : UIBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField]
    public int startingIndex = 0;
    [SerializeField]
    public bool wrapAround = false;
    [SerializeField]
    public float lerpTimeMilliSeconds = 200f;
    [SerializeField]
    public float triggerPercent = 5f;
    [Range(0f, 10f)]
    public float triggerAcceleration = 1f;

    [SerializeField]
    private Button previousButton = null;
    [SerializeField]
    private Button nextButton = null;

    public class OnLerpCompleteEvent : UnityEvent { }
    public OnLerpCompleteEvent onLerpComplete;
    public class OnReleaseEvent : UnityEvent<int> { }
    public OnReleaseEvent onRelease;

    private int actualIndex;
    [SerializeField]
    private int cellIndex;
    private ScrollRect scrollRect;
    private CanvasGroup canvasGroup;
    private RectTransform content;
    private Vector2 cellSize;
    private bool indexChangeTriggered = false;
    private bool isLerping = false;
    private DateTime lerpStartedAt;
    private Vector2 releasedPosition;
    private Vector2 targetPosition;

    protected override void Awake()
    {
        base.Awake();
        actualIndex = startingIndex;
        cellIndex = startingIndex;
        onLerpComplete = new OnLerpCompleteEvent();
        onRelease = new OnReleaseEvent();
        scrollRect = GetComponent<ScrollRect>();
        canvasGroup = GetComponent<CanvasGroup>();
        content = scrollRect.content;
        cellSize = content.GetComponent<GridLayoutGroup>().cellSize;
        content.anchoredPosition = new Vector2(-cellSize.x * cellIndex, content.anchoredPosition.y);
        int count = LayoutElementCount();
        SetContentSize(count);

        if (startingIndex < count)
        {
            MoveToIndex(startingIndex);
        }
    }

    protected override void Start()
    {
        previousButton.onClick.AddListener(SnapToPrev);
        nextButton.onClick.AddListener(SnapToNext);
    }

    void LateUpdate()
    {
        if (isLerping)
        {
            LerpToElement();
            if (ShouldStopLerping())
            {
                isLerping = false;
                canvasGroup.blocksRaycasts = true;
                onLerpComplete.Invoke();
                onLerpComplete.RemoveListener(WrapElementAround);
            }
        }
    }

    public void PushLayoutElement(LayoutElement element)
    {
        element.transform.SetParent(content.transform, false);
        SetContentSize(LayoutElementCount());
    }

    public void PopLayoutElement()
    {
        LayoutElement[] elements = content.GetComponentsInChildren<LayoutElement>();
        Destroy(elements[elements.Length - 1].gameObject);
        SetContentSize(LayoutElementCount() - 1);
        if (cellIndex == CalculateMaxIndex())
        {
            cellIndex -= 1;
        }
    }

    public void UnshiftLayoutElement(LayoutElement element)
    {
        cellIndex += 1;
        element.transform.SetParent(content.transform, false);
        element.transform.SetAsFirstSibling();
        SetContentSize(LayoutElementCount());
        content.anchoredPosition = new Vector2(content.anchoredPosition.x - cellSize.x, content.anchoredPosition.y);
    }

    public void ShiftLayoutElement()
    {
        Destroy(GetComponentInChildren<LayoutElement>().gameObject);
        SetContentSize(LayoutElementCount() - 1);
        cellIndex -= 1;
        content.anchoredPosition = new Vector2(content.anchoredPosition.x + cellSize.x, content.anchoredPosition.y);
    }

    public int LayoutElementCount()
    {
        return content.GetComponentsInChildren<LayoutElement>(false)
            .Count(e => e.transform.parent == content);
    }

    public int CurrentIndex
    {
        get
        {
            int count = LayoutElementCount();
            int mod = actualIndex % count;
            return mod >= 0 ? mod : count + mod;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        float dx = data.delta.x;
        float dt = Time.deltaTime * 1000f;
        float acceleration = Mathf.Abs(dx / dt);
        if (acceleration > triggerAcceleration && acceleration != Mathf.Infinity)
        {
            indexChangeTriggered = true;
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (IndexShouldChangeFromDrag(data))
        {
            int direction = (data.pressPosition.x - data.position.x) > 0f ? 1 : -1;
            SnapToIndex(cellIndex + direction * CalculateScrollingAmount(data));
        }
        else
        {
            StartLerping();
        }
    }

    public int CalculateScrollingAmount(PointerEventData data)
    {
        var offset = scrollRect.content.anchoredPosition.x + cellIndex * cellSize.x;
        var normalizedOffset = Mathf.Abs(offset / cellSize.x);
        var skipping = (int)Mathf.Floor(normalizedOffset);
        if (skipping == 0)
            return 1;
        if ((normalizedOffset - skipping) * 100f > triggerPercent)
        {
            return skipping + 1;
        }
        else
        {
            return skipping;
        }
    }

    public void SnapToNext()
    {
        SnapToIndex(cellIndex + 1);
    }

    public void SnapToPrev()
    {
        SnapToIndex(cellIndex - 1);
    }

    public void SnapToIndex(int newCellIndex)
    {
        int maxIndex = CalculateMaxIndex();
        if (wrapAround && maxIndex > 0)
        {
            actualIndex += newCellIndex - cellIndex;
            cellIndex = newCellIndex;
            onLerpComplete.AddListener(WrapElementAround);
        }
        else
        {
            newCellIndex = Mathf.Clamp(newCellIndex, 0, maxIndex);
            actualIndex += newCellIndex - cellIndex;
            cellIndex = newCellIndex;
        }

        UpdateNavigationButtons();

        onRelease.Invoke(cellIndex);
        StartLerping();
    }

    public void UpdateNavigationButtons()
    {
        StartCoroutine(RefreshNavButtons());
    }

    public void MoveToIndex(int newCellIndex)
    {
        int maxIndex = CalculateMaxIndex();
        if (newCellIndex >= 0 && newCellIndex <= maxIndex)
        {
            actualIndex += newCellIndex - cellIndex;
            cellIndex = newCellIndex;
        }

        UpdateNavigationButtons();

        onRelease.Invoke(cellIndex);
        content.anchoredPosition = CalculateTargetPoisition(cellIndex);
    }

    public void ClearElements()
    {
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        nextButton.gameObject.SetActive(false);
        previousButton.gameObject.SetActive(false);
    }

    void StartLerping()
    {
        releasedPosition = content.anchoredPosition;
        targetPosition = CalculateTargetPoisition(cellIndex);
        lerpStartedAt = DateTime.Now;
        canvasGroup.blocksRaycasts = false;
        isLerping = true;
    }

    int CalculateMaxIndex()
    {
        int cellPerFrame = Mathf.FloorToInt(scrollRect.GetComponent<RectTransform>().rect.size.x / cellSize.x);
        return LayoutElementCount() - cellPerFrame;
    }

    bool IndexShouldChangeFromDrag(PointerEventData data)
    {
        // acceleration was above threshold
        if (indexChangeTriggered)
        {
            indexChangeTriggered = false;
            return true;
        }
        // dragged beyond trigger threshold
        var offset = scrollRect.content.anchoredPosition.x + cellIndex * cellSize.x;
        var normalizedOffset = Mathf.Abs(offset / cellSize.x);
        return normalizedOffset * 100f > triggerPercent;
    }

    void LerpToElement()
    {
        float t = (float)((DateTime.Now - lerpStartedAt).TotalMilliseconds / lerpTimeMilliSeconds);
        float newX = Mathf.Lerp(releasedPosition.x, targetPosition.x, t);
        content.anchoredPosition = new Vector2(newX, content.anchoredPosition.y);
    }

    void WrapElementAround()
    {
        if (cellIndex <= 0)
        {
            var elements = content.GetComponentsInChildren<LayoutElement>();
            elements[elements.Length - 1].transform.SetAsFirstSibling();
            cellIndex += 1;
            content.anchoredPosition = new Vector2(content.anchoredPosition.x - cellSize.x, content.anchoredPosition.y);
        }
        else if (cellIndex >= CalculateMaxIndex())
        {
            var element = content.GetComponentInChildren<LayoutElement>();
            element.transform.SetAsLastSibling();
            cellIndex -= 1;
            content.anchoredPosition = new Vector2(content.anchoredPosition.x + cellSize.x, content.anchoredPosition.y);
        }
    }

    void SetContentSize(int elementCount)
    {
        content.sizeDelta = new Vector2(cellSize.x * elementCount, content.rect.height);
    }

    Vector2 CalculateTargetPoisition(int index)
    {
        return new Vector2(-cellSize.x * index, content.anchoredPosition.y);
    }

    bool ShouldStopLerping()
    {
        return Mathf.Abs(content.anchoredPosition.x - targetPosition.x) < 0.001;
    }

    IEnumerator RefreshNavButtons()
    {
        yield return null;
        int elemcount = content.childCount;
        nextButton.gameObject.SetActive(cellIndex < elemcount - 1);
        previousButton.gameObject.SetActive(cellIndex > 0);
    }
}
