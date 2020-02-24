namespace Items.Shops.UI {
	using LSW.Tooltip;
	using UnityEngine;

	public class BasketItemInfoArea : TooltipArea<SimpleSmallTooltip.Data> {
		[SerializeField] private BasketItemSlotUI basketItem = null;

		protected override SimpleSmallTooltip.Data TooltipData => new SimpleSmallTooltip.Data { text = basketItem.currentItem.Description };
	}
}