using PlayerStats.Buffs.Core;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerStats.Buffs
{
    [CreateAssetMenu(fileName = "Deep Sleep", menuName = "Buffs/DeepSleep", order = 0)]
    public class DeepSleep : Buff<DeepSleep>
    {
        public override void TakeEffect()
        { }

        public override void TakeEffect(BuffArgs args)
        { }

        public override void End()
        { }
    }
}