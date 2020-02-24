using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventorySystem
{
    public class TooltipArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private const float ShowDelay = 0.2f;
        private bool onHover;

        new private string name = "Name";
        private string description = "Description";

        public void SetDisplay(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onHover = true;
            Invoke(nameof(ShowToolTip), ShowDelay);
        }

        private void ShowToolTip()
        {
            if (onHover)
                ItemToolTip.Show(name, description);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onHover = false;
            CancelInvoke(nameof(ShowToolTip));
            ItemToolTip.Hide();
        }
    }
}