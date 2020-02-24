using System;
using NSubstitute;
using PlayerStats.Buffs.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerStats.Buffs
{
    [CreateAssetMenu(fileName = "Suppression", menuName = "Buffs/Suppression", order = 0)]
    public class Suppression : Buff<Suppression>
    {
        [BoxGroup("Effect Details")]
        [LabelText("Decrease health loss by (%)")]
        [SerializeField]
        private float percentDecrease = 50f;
        
        public override void TakeEffect()
        { }

        public override void TakeEffect(BuffArgs args)
        {
            if (args.GetType() == typeof(StatDecreaseArgs))
                UpdateStat((StatDecreaseArgs) args);
        }
        
        private void UpdateStat(StatDecreaseArgs statsArgs)
        {
            statsArgs.DecreaseValue(percentDecrease);
        }
        
        public override void End()
        { }
    }
}