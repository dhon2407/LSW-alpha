using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GUI_Animations
{
    [RequireComponent(typeof(Image))]
    public class DraggableHandle : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
    {
        [SerializeField] private RectTransform mainWindow = null;
        [SerializeField] private Canvas canvas = null;
        [SerializeField] private CanvasGroup background = null;

        public void OnDrag(PointerEventData eventData)
        {
            mainWindow.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            background.alpha = 0.4f;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            background.alpha = 1f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            mainWindow.SetAsLastSibling();
        }

        private void Reset()
        {
            GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }

        private void Start()
        {
            Reset();
        }
    }
}