using LSW.Tooltip;
using PlayerStats.Buffs.Core;
using UI.Buffs;
using UnityEngine;

namespace PlayerStats.Buffs
{
    public class BuffTooltipArea : TooltipArea<IBuff>
    {
        [SerializeField] private BuffItem buffItem = null;

        private void Awake()
        {
            if (buffItem == null)
                buffItem = GetComponentInParent<BuffItem>();
        }

        protected override IBuff TooltipData => buffItem.CurrentBuff;
    }
}