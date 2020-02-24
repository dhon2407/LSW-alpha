using LSW.Tooltip;
using UnityEngine;

using static PlayerStats.Skill.Type;

namespace PlayerStats
{
    public class StatTooltipArea : TooltipArea<StatTooltip.Data>
    {
        [SerializeField] private Skill.Type statType = Unknown;
        
        protected override StatTooltip.Data TooltipData => new StatTooltip.Data
        {
            statType =  statType,
            xpCurrent =  (statType == Unknown) ? 0 : Stats.Skill(statType).GetData().xp,
            xpMax = (statType == Unknown) ? float.MaxValue : Stats.Skill(statType).GetData().RequiredXP,
        };
    }
}