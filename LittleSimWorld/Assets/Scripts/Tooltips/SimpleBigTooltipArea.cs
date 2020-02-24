using UnityEngine;

namespace LSW.Tooltip
{
    public class SimpleBigTooltipArea : TooltipArea<SimpleBigTooltip.Data>
    {
        public string tooltipDescription = string.Empty;

        protected override SimpleBigTooltip.Data TooltipData =>
            new SimpleBigTooltip.Data {text = tooltipDescription};
    }
}