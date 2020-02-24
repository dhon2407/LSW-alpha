using PlayerStats.Buffs.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerStats.Buffs
{
    [CreateAssetMenu(fileName = "Productive", menuName = "Buffs/Productive", order = 0)]
    public class Productive : Buff<Productive>
    {
        [BoxGroup("Effect Details")]
        [LabelText("Increase all experience gain by (%)")]
        [SerializeField]
        private float percentIncrease = 15f;
        
        public override void TakeEffect()
        { }

        public override void TakeEffect(BuffArgs args)
        {
            if (args.GetType() == typeof(StatIncreaseArgs))
                UpdateStat((StatIncreaseArgs) args);
        }

        private void UpdateStat(StatIncreaseArgs args)
        {
            args.IncreaseValue(percentIncrease);
        }

        public override void End()
        { }
    }
}