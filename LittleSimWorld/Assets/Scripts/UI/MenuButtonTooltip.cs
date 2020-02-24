using LSW.Tooltip;
using UnityEngine;

namespace UI
{
    public class MenuButtonTooltip : TooltipArea<SimpleBigTooltip.Data>
    {
        [SerializeField] private string description = string.Empty;

        public void SetShortcutKey(KeyCode code)
        {
            var keyname = code.ToString();
            if (code == KeyCode.Escape)
                keyname = "Esc";
            
            _shortcutKey = $" ({keyname})";
        }
        
        
        private string _shortcutKey = string.Empty;
        
        protected override SimpleBigTooltip.Data TooltipData =>
            new SimpleBigTooltip.Data {text = description + _shortcutKey};
    }
}