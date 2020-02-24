using System;
using Items.Shops.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Zenject;

namespace LSW.Tooltip
{
    public abstract class TooltipArea<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected ITooltip<T> ToolTip;
        protected UnityEvent onMouseEnter;
        protected UnityEvent onMouseExit;

        private bool _mouseHover;

        [Inject]
        public void Init(ITooltip<T> tooltip)
        {
            ToolTip = tooltip;

            onMouseEnter = new UnityEvent();
            onMouseExit = new UnityEvent();
        }

        protected abstract T TooltipData { get; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onMouseEnter?.Invoke();
            ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onMouseExit?.Invoke();
            HideTooltip();
        }

        public void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            onMouseEnter?.Invoke();
            ShowTooltip();
        }

        public void OnMouseExit()
        {
            onMouseExit?.Invoke();
            HideTooltip();
        }

        public void OnMouseOver()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
                ShowTooltip();
            else
                HideTooltip();
        }

        private void ShowTooltip()
        {
            ToolTip.SetData(TooltipData);
            ToolTip.Show();
            _mouseHover = true;
        }

        private void HideTooltip()
        {
            ToolTip.Hide();
            _mouseHover = false;
        }

        private void OnDestroy()
        {
            if (_mouseHover)
                HideTooltip();
        }

        private void OnDisable()
        {
            if (_mouseHover)
                HideTooltip();
        }
    }
}