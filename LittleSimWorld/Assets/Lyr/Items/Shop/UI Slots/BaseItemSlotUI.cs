namespace Items.Shops.UI {
    using LSW.Tooltip;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    /// <summary>
    /// Base class for UI Slots that represent items.
    /// </summary>
    public abstract class BaseItemSlotUI : TooltipArea<SimpleBigTooltip.Data> {
        [SerializeField] protected new TMPro.TextMeshProUGUI name = null;
        [SerializeField] protected Image icon = null;
        [SerializeField] protected Button actionButton = null;

        public IShopItem currentItem;

   public void SetAction(UnityAction action) => actionButton.onClick.AddListener(action);
        protected override SimpleBigTooltip.Data TooltipData => new SimpleBigTooltip.Data { text = currentItem.Name };
    }
}